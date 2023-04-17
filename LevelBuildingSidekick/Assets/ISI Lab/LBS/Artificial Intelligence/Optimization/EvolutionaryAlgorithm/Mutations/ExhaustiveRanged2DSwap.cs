using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustiveRanged2DSwap : MutationBase
{
    int range = 1;

    public ExhaustiveRanged2DSwap(int range)
    {
        this.range = range;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var chr = chromosome as ChromosomeBase2D;

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
                    var j = i + chr.WorldToIndex(pos);
                    if (chromosome.IsImmutable(j))
                        continue;
                    if (j < chr.Length)
                    {
                        var aux = chr.GetGene(i);
                        chr.ReplaceGene(i, chr.GetGene(j));
                        chr.ReplaceGene(j, aux);
                    }
                }
            }
        }
    }
}
