using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapGene : MutationBase
{
    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var i = r.GetInt(0, chromosome.Length);
        var j = r.GetInt(0, chromosome.Length);

        var aux = chromosome.GetGene(i);
        chromosome.ReplaceGene(i, chromosome.GetGene(j));
        chromosome.ReplaceGene(j, aux);
    }
}
