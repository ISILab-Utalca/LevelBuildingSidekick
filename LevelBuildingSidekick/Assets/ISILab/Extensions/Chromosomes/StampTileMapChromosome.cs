using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using Utility;

public class StampTileMapChromosome : ChromosomeBase2D<int>, IDrawable
{
    public List<StampData> stamps { get; private set; }
    private int tileSize;

    public StampTileMapChromosome(LBSStampTileMapController stampController) : base(4, 2)
    {
        var rawStamps = (stampController.GetData() as LBSStampGroupData).GetStamps();

        tileSize = (int)stampController.TileSize;

        var x1 = rawStamps.Min(s => s.Position.x);
        var x2 = rawStamps.Max(s => s.Position.x);

        var y1 = rawStamps.Min(s => s.Position.y);
        var y2 = rawStamps.Max(s => s.Position.y);

        int width = (x2 - x1) + 1;
        int height = (y2 - y1) + 1;


        var size = new Vector2Int(width, height);
        var offset = new Vector2(x1, y1);

        Resize(size.y * size.x);

        MatrixWidth = (int)size.x;

        List<string> reviwed = new List<string>();
        stamps = rawStamps.Where(s =>
        {
            if(reviwed.Contains(s.Label))
            {
                return false;
            }
            reviwed.Add(s.Label);
            return true;
        }).ToList();

        for(int i = 0; i < Length; i++)
        {
            ReplaceGene(i, -1);
        }

        Debug.Log(Length);

        foreach (var stamp in rawStamps)
        {
            var index = ToIndex(stamp.Position - offset);
            ReplaceGene(index, stamps.FindIndex(s => s == stamp));
        }

    }

    public StampTileMapChromosome(int length, int matrixWidth, List<StampData> stamps) : base(length, matrixWidth)
    {
        this.stamps = stamps.Select(s => s).ToList();
        for (int i = 0; i < Length; i++)
        {
            ReplaceGene(i, -1);
        }

    }
        
    public override IChromosome CreateNewChromosome()
    {
        var c = new StampTileMapChromosome(Length, MatrixWidth, stamps);
        c.tileSize = tileSize;
        return c;
    }

    public override object GenerateGene(int geneIndex)
    {
        return RandomizationProvider.Current.GetInt(-1, stamps.Count);
    }

    public override Texture2D ToTexture()
    {
        int width = MatrixWidth * tileSize;
        int height = (Length / MatrixWidth) * tileSize;

        Texture2D texture = new Texture2D(width, height);

        Texture2D empty = new Texture2D(1,1);
        empty.SetPixel(0, 0, new Color(0,0,0,0));
        empty.Apply();

        string s = "";

        for(int i = 0; i < Length; i++)
        {
            var pos = ToMatrixPosition(i);
            var id = GetGene<int>(i);
            if (id == -1)
            {
                texture.InsertTextureInRect(empty, (int)pos.x * tileSize, (int)pos.y * tileSize, tileSize, tileSize);
            }
            else
            {
                var t = DirectoryTools.GetScriptable<StampPresset>(stamps[id].Label).Icon;
                texture.InsertTextureInRect(t, (int)pos.x*tileSize, (int)pos.y*tileSize, tileSize, tileSize);
            }
        }

        texture.Apply();
        return texture;
    }
}
