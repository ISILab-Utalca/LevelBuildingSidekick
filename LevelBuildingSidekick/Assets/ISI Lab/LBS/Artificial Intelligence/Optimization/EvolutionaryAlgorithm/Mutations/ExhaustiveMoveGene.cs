using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ExhaustiveMoveGene : MutationBase
{
    [SerializeField]
    int range = 1;
    public int Range
    {
        get => range;
        set
        {
            if (value <= 1)
                range = 1;
            range = value;
        }
    }

    public ExhaustiveMoveGene()
    {
        this.range = 1;
    }

    public ExhaustiveMoveGene(int range)
    {
        this.range = range;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var chr = chromosome as BundleTilemapChromosome;

        if (chr == null)
        {
            throw new Exception("Chromosome must inherit from ChromosomeBase2D");
        }

        for (int i = 0; i < chr.Length; i++)
        {
            if (chromosome.IsImmutable(i))
                continue;
            if (chr.GetGene(i) != default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    var pos = new Vector2Int(r.GetInt(-range, range), r.GetInt(-range, range));
                    var j = i + chr.ToIndex(pos);
                    if (j < chr.Length && j >= 0)
                    {
                        if (chromosome.IsImmutable(j))
                            continue;

                        chr.ReplaceGene(j, chr.GetGene(i));
                        chr.SetDeafult(i);
                    }
                }
            }
        }
    }
}
