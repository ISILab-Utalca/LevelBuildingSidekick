using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using LBS.Components;
using LBS.Components.TileMap;
using Utility;

[ChromosomeFromModule(typeof(TaggedTileMap))]
public class TaggedTileMapChromosome : ChromosomeBase2D
{
    public TaggedTileMapChromosome(LBSModule module) : base()
    {
        var tileMap = module as TaggedTileMap;

        if(module == null)
        {
            throw new System.Exception("[ISI Lab] Class must be TaggedTileMap");
        }

        Rect = tileMap.Rect;

        genes = new object[(int)(Rect.width * Rect.height)];


        var tiles = tileMap.PairTiles.Select(x => x.tile);

        foreach (var t in tiles)
        {
            var i = ToIndex(t.Position);
            var data = tileMap.GetBundleData(t);
            ReplaceGene(i, data);
        }

    }

    public TaggedTileMapChromosome(Rect rect, int length) : base(rect)
    {
        genes = new object[length];
    }

    public override IChromosome CreateNewChromosome()
    {
        var chrom = new TaggedTileMapChromosome(Rect, Length);
        for(int i = 0; i < Length; i++)
        {
            chrom.ReplaceGene(i, GenerateGene());
        }
        return chrom;
    }

    public override object GenerateGene()
    {
        return (GetGene(RandomizationProvider.Current.GetInt(0, Length)) as BundleData)?.Clone();
    }

    public override void SetDeafult(int index)
    {
        ReplaceGene<BundleData>(index, null);
    }

    public override Texture2D ToTexture()
    {
        int tSize = 16;
        var texture = new Texture2D((int)Rect.width* tSize, (int)Rect.height* tSize);
        for(int i = 0; i < genes.Length; i++)
        {
            var pos = ToMatrixPosition(i);
            if (genes[i] == null)
            {
                var t = new Texture2D(1, 1);
                t.SetPixel(0, 0, new Color(0, 0, 0, 0));
                texture.InsertTextureInRect(t, pos.x* tSize, pos.y* tSize, tSize, tSize);
            }
            else
            {
                var t = Utility.DirectoryTools.GetScriptable<LBSIdentifier>((genes[i] as BundleData).BundleTag).Icon;
                texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
            }
        }
        texture.Apply();
        return texture;
    }
}
