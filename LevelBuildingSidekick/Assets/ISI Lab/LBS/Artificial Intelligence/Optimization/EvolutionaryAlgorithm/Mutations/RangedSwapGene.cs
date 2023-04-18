using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSwapGene : MutationBase
{
    int range = 1;

    public RangedSwapGene()
    {

    }

    public RangedSwapGene(int range)
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

            var i = r.GetInt(0, chromosome.Length);

        while (i < chromosome.Length && chromosome.GetGene(i) == default)
        {
            i++;
        }

        if (i < chromosome.Length)
        {
            var j = Mathf.Clamp(i + r.GetInt(-range, range + 1), 0, chromosome.Length - 1);

            var aux = chromosome.GetGene(i);
            chromosome.ReplaceGene(i, chromosome.GetGene(j));
            chromosome.ReplaceGene(j, aux);
        }

    }
}
