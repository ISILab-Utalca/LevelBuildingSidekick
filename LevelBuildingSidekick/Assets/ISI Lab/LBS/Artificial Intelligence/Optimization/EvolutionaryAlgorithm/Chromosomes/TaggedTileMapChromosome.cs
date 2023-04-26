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
public class TaggedTileMapChromosome : LBSChromosome, IDrawable
{
    public TaggedTileMapChromosome(LBSModule module, Rect rect, int[] immutables = null) : base(rect, immutables)
    {
        var tileMap = module as TaggedTileMap;

        if(module == null)
        {
            throw new System.Exception("[ISI Lab] Class must be TaggedTileMap");
        }

        var tiles = tileMap.PairTiles.Select(x => x.tile);

        foreach (var t in tiles)
        {
            var i = WorldToIndex(t.Position);
            var data = tileMap.GetBundleData(t);
            ReplaceGene(i, data);
        }

    }

    public TaggedTileMapChromosome(Rect rect, int length, int[] immutables = null) : base(rect, immutables)
    {
        Rect = rect;
        genes = new object[length];
    }

    public override ChromosomeBase CloneChromosome()
    {
        var chrom = new TaggedTileMapChromosome(Rect, Length, immutableIndexes);
        for (int i = 0; i < Length; i++)
        {
            if (IsImmutable(i))
                continue;
            chrom.ReplaceGene(i, GetGene(i));
        }
        return chrom;
    }

    public override ChromosomeBase CreateNewChromosome()
    {
        var chrom = new TaggedTileMapChromosome(Rect, Length, immutableIndexes);
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

    public override bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public override void SetDeafult(int index)
    {
        ReplaceGene<BundleData>(index, null);
    }

    public override LBSModule ToModule()
    {

        var tiles = new List<TileBundlePair>(); 

        for(int i = 0; i < Length; i++)
        {
            if(genes[i] != null)
            {
                var pos = ToMatrixPosition(i) + Rect.position;
                var t = new LBSTile(pos, "Tile: " + pos);
                tiles.Add(new TileBundlePair(t, genes[i] as BundleData));
            }
        }

        var mod = new TaggedTileMap("C_TaggedTileMap",tiles);

        return mod;
    }

    public Texture2D ToTexture()
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
                var source = Utility.DirectoryTools.GetScriptable<LBSIdentifier>((genes[i] as BundleData).BundleTag).Icon;
                var t = new Texture2D(source.width, source.height);
                t.SetPixels(source.GetPixels());
                t.MirrorY();
                t.Apply();
                texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
            }
        }
        texture.MirrorY();
        texture.Apply();
        return texture;
    }
}
