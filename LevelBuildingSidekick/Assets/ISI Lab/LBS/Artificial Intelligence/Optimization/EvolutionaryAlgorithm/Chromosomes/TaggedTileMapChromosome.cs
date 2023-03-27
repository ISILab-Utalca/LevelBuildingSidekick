using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using LBS.Components;

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

        genes = new object[(int)(tileMap.Rect.width * tileMap.Rect.height)];

        foreach(var t in tileMap.tiles.Keys)
        {
            ReplaceGene(ToIndex(t.Position), tileMap.tiles[t]);
        }
    }

    public TaggedTileMapChromosome(int matrixWidth, int length) : base(matrixWidth)
    {
        genes = new object[length];
    }

    public override IChromosome CreateNewChromosome()
    {
        return new TaggedTileMapChromosome(MatrixWidth, Length);
    }

    public override object GenerateGene()
    {
        throw new System.NotImplementedException();
    }

    public override void SetDeafult(int index)
    {
        throw new System.NotImplementedException();
    }

    public override Texture2D ToTexture()
    {
        throw new System.NotImplementedException();
    }
}
