using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using LBS.Components.TileMap;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Modules;

public class BundleTilemapChromosome : ChromosomeBase2D, IDrawable
{
    public BundleTilemapChromosome(BundleTileMap tileMap, Rect rect, int[] immutables = null) : base(rect, immutables)
    {
        var tiles = tileMap.Tiles;

        foreach (var t in tiles)
        {
            if (!rect.Contains(t.Tile.Position))
                continue;
            var i = ToIndex(t.Tile.Position - rect.position);
            var data = t.BundleData.Clone() as BundleData;
            ReplaceGene(i, data);
        }
    }

    public BundleTilemapChromosome(Rect rect, int[] immutables = null) : base(rect, immutables) { }

    public override ChromosomeBase CloneChromosome()
    {
        var chrom = new BundleTilemapChromosome(Rect, immutableIndexes);
        for (int i = 0; i < Length; i++)
        {
            chrom.ReplaceGene(i, GetGene(i));
        }
        return chrom;
    }

    public override ChromosomeBase CreateNewChromosome()
    {
        var chrom = new BundleTilemapChromosome(Rect, immutableIndexes);
        for(int i = 0; i < Length; i++)
        {
            if (!IsImmutable(i))
                chrom.ReplaceGene(i, GenerateGene());
            else
                chrom.ReplaceGene(i, GetGene(i));
        }
        return chrom;
    }

    public override object GenerateGene()
    {
        int index = -1;
        do
        {
            index = RandomizationProvider.Current.GetInt(0, Length);
        }
        while (IsImmutable(index));

        return (GetGene(index) as BundleData)?.Clone();
    }

    public override bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public override void SetDeafult(int index)
    {
        ReplaceGene<BundleData>(index, null);
    }

    public Texture2D ToTexture()
    {
        int tSize = 16;

        var texture = new Texture2D((int)Rect.width * tSize, (int)Rect.height * tSize);

        for(int i = 0; i < genes.Length; i++)
        {
            var pos = ToMatrixPosition(i);
            if (genes[i] == null)
            {
                var t = new Texture2D(1, 1);
                t.SetPixel(0, 0, new Color(0, 0, 0, 0));
                texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
            }
            else
            {
                var source = (genes[i] as BundleData).Bundle.Icon;
                var color = (genes[i] as BundleData).Bundle.Color;
                var t = new Texture2D(source.width, source.height);
                t.SetAllPixels(color);
                t = t.MergeTextures(source);
                texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
            }
        }
        texture.Apply();
        return texture;
    }
}
