using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustiveSwapGene : MutationBase
{
    protected override void PerformMutate(IChromosome chromosome, float probability)
    {
        var r = RandomizationProvider.Current;


        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome.GetGene(i) != default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    var j = r.GetInt(0, chromosome.Length);

                    var aux = chromosome.GetGene(i);
                    chromosome.ReplaceGene(i, chromosome.GetGene(j));
                    chromosome.ReplaceGene(j, aux);
                }
            }
        }
    }
}
