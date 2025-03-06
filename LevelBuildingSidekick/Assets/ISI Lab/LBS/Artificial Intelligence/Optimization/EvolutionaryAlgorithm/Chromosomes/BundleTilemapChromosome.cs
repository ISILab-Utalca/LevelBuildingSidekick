using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Categorization
{
    public class BundleTilemapChromosome : ChromosomeBase2D, IDrawable
    {
        public BundleTilemapChromosome(BundleTileMap tileMap, Rect rect, int[] immutables = null) : base(rect, immutables)
        {
            var groups = tileMap.Groups;

            foreach(var group in groups) { 
                foreach (var tile in group.TileGroup)
                {
                    if (!rect.Contains(tile.Position))
                        continue;
                    var i = ToIndex(tile.Position - rect.position);
                    var data = group.BundleData.Clone() as BundleData;
                    ReplaceGene(i, data);
                }
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
            for (int i = 0; i < Length; i++)
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

            for (int i = 0; i < genes.Length; i++)
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
                    Texture2D source = (genes[i] as BundleData).Bundle.Icon;
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
}