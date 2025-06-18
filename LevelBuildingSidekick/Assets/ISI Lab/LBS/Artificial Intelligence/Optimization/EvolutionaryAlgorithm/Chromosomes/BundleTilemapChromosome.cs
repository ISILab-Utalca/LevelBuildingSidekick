using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using ISILab.AI.Optimization;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Categorization
{
    public class BundleTilemapChromosome : ChromosomeBase2D, IDrawable, IChromosome
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

        public bool IsEmpty()
        {
            var geneList = GetGenes().Cast<object>().ToList();
            var NonNullGenes = geneList.FindAll(b => b != null);
            return NonNullGenes.Count() == immutableIndexes.Length;

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
                    // Create a transparent 1x1 texture
                    var t = new Texture2D(1, 1);
                    t.SetPixel(0, 0, new Color(0, 0, 0, 0));
                    t.Apply();
                    texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
                }
                else
                {
                    // Retrieve the VectorImage and the color
                    VectorImage vectorImage = (genes[i] as BundleData).Bundle.Icon;
                    var color = (genes[i] as BundleData).Bundle.Color;

                    // Convert VectorImage to Texture2D
                    Texture2D source = ConvertVectorImageToTexture(vectorImage, tSize, tSize, color);

                    // Insert it into the final texture
                    texture.InsertTextureInRect(source, pos.x * tSize, pos.y * tSize, tSize, tSize);
                }
            }

            texture.Apply();
            return texture;
            

            /* OLD VERSION
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
                    Texture2D source = (genes[i] as BundleData).Bundle.Icon; // icon is a vector image!
                    var color = (genes[i] as BundleData).Bundle.Color;
                    var t = new Texture2D(source.width, source.height);
                    t.SetAllPixels(color);
                    t = t.MergeTextures(source);
                    texture.InsertTextureInRect(t, pos.x * tSize, pos.y * tSize, tSize, tSize);
                }
            }
            texture.Apply();
            return texture;
            */
        }
        
        private Texture2D ConvertVectorImageToTexture(VectorImage vectorImage, int width, int height, Color color)
        {
            // Create a RenderTexture to draw on
            RenderTexture renderTexture = new RenderTexture(width, height, 32);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            // Create a Texture2D to store the result
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

            // Draw the VectorImage onto the RenderTexture
            Graphics.Blit(null, renderTexture);

            // Read the RenderTexture into the Texture2D
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
    
            // Cleanup
            RenderTexture.active = null;
            renderTexture.Release();

            // Apply the color
            Color[] pixels = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        /*public BundleTileMap ToTileMap()
        {
            var newTileMap = new BundleTileMap();
            for(int i=0; i<genes.Length; i++)
            {
                if(genes[i]!=null)
                {
                    var geneData = genes[i] as BundleData;
                    newTileMap.AddGroup(new TileBundleGroup(this.ToMatrixPosition(i), geneData.Bundle.TileSize, geneData, Vector2.right));
                }
            }
            return newTileMap;
        }*/

        IChromosome IChromosome.CreateNewChromosome()
        {
            throw new System.NotImplementedException();
        }

        IChromosome IChromosome.CloneChromosome()
        {
            throw new System.NotImplementedException();
        }

        public int CompareTo(IChromosome other)
        {
            throw new System.NotImplementedException();
        }
    }
}