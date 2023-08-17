using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged2DSwapMutation : MutationBase
{
    int range = 1;

    public Ranged2DSwapMutation()
    {

    }

    public Ranged2DSwapMutation(int range)
    {
        this.range = range;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var d = r.GetDouble();
        if (d > probability)
        {
            return;
        }

        var chr = chromosome as ChromosomeBase2D;

        if(chr == null)
        {
            throw new Exception("Chromosome must inherit from ChromosomeBase2D");
        }

        var i = r.GetInt(0, chr.Length - range);

        while (i < chr.Length && chr.GetGene(i) == default)
        {
            i++;
        }

        var pos = new Vector2Int(r.GetInt(-range, range), r.GetInt(-range, range));
        var j = i + chr.ToIndex(pos);

        if (j < chr.Length)
        {
            var aux = chromosome.GetGene(i);
            chromosome.ReplaceGene(i, chromosome.GetGene(j));
            chromosome.ReplaceGene(j, aux);
        }

    }
}
