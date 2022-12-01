using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using Utility;
using LBS.Representation.TileMap;

public class StampTileMapChromosome : ChromosomeBase2D<int>, IDrawable
{
    public static LBSTileMapController TileMap { get; set; }

    public List<StampData> stamps { get; private set; }
    private int tileSize;

    public StampTileMapChromosome(LBSStampTileMapController stampController) : base(4, 2)
    {
        var rawStamps = (stampController.GetData() as LBSStampGroupData).GetStamps();

        tileSize = (int)stampController.TileSize;

        var tileMap = TileMap.GetData() as LBSSchemaData;
        var tiles = tileMap.GetRooms().SelectMany(r => r.TilesPositions);

        var x1 = tiles.Min(t => t.x);
        var x2 = tiles.Max(t => t.x);

        var y1 = tiles.Min(t => t.y);
        var y2 = tiles.Max(t => t.y);

        int width = (x2 - x1) + 1;
        int height = (y2 - y1) + 1;

        var size = new Vector2Int(width, height);
        var offset = new Vector2(x1, y1);

        Resize(size.y * size.x);

        MatrixWidth = width;

        List<string> reviwed = new List<string>();
        stamps = rawStamps.Where(s =>
        {
            if (reviwed.Contains(s.Label))
            {
                return false;
            }
            reviwed.Add(s.Label);
            return true;
        }).ToList();

        for (int i = 0; i < Length; i++)
        {
            base.ReplaceGene(i, -1);
        }

        List<int> incidences = new List<int>();

        foreach (var stamp in rawStamps)
        {
            var index = ToIndex(stamp.Position - offset);
            var id = stamps.FindIndex(s => s.Label == stamp.Label);
            //Debug.Log("TP: " + (stamp.Position - offset) + " - Index: " + index + " - MP: " + ToMatrixPosition(index) + " - ID: " + id);
            incidences.Add(index);
            ReplaceGene(index, id);
        }
        Debug.Log("Incidences: " + incidences.Count);
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
        int tsize = 16;
        int width = MatrixWidth;
        int height = (Length / MatrixWidth);
        var size = width > height ? width : height;

        Texture2D texture = new Texture2D(size * tsize, size * tsize);

        Texture2D empty = new Texture2D(1, 1);
        empty.SetPixel(0, 0, new Color(0, 0, 0, 0));
        empty.Apply();

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var index = ToIndex(new Vector2(i, j));
                var id = GetGene<int>(index);
                if (id == -1)
                {
                    texture.InsertTextureInRect(empty, i * tsize, (height - 1 - j) * tsize, tsize, tsize);
                }
                else
                {
                    var t = DirectoryTools.GetScriptable<StampPresset>(stamps[id].Label).Icon;
                    texture.InsertTextureInRect(t, i * tsize, (height - 1 - j) * tsize, tsize, tsize);
                }
            }
        }
        texture.Apply(); 

        return texture;
    }

    public override void ReplaceGene<T>(int index, T gene)
    {
        if (!(gene is int))
        {
            Debug.LogError("Incorrect Data type");
            return;
        }

        int i = (int)(object)gene;

        if(i < -1 || i >= stamps.Count)
        {
            Debug.LogError("Out of Bounds");
            return;
        }

        var tileMap = TileMap.GetData() as LBSSchemaData;
        var tiles = tileMap.GetRooms().SelectMany(r => r.TilesPositions);

        string s = ""; 
        if (i != -1)
        {
            s += "Index: " + index + " - Pos: " + ToMatrixPosition(index) + " // ";
            if (!tiles.Any(t => t == ToMatrixPosition(index)))
            {
                base.ReplaceGene(index, -1);
                s += "Rejected";
                //Debug.Log(s);
                return;
            }
            s += "Approved";
        }
        //Debug.Log(s);

        base.ReplaceGene(index, gene);
    }
}
