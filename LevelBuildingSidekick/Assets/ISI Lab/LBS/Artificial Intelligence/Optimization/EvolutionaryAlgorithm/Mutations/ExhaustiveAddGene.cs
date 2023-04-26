using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExhaustiveAddGene : MutationBase
{
    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;
        var genes = chromosome.GetGenes().Where(g => g != null).Distinct().ToList();

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome.IsImmutable(i))
                continue;
            if(chromosome.GetGene(i) != default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    chromosome.ReplaceGene(i, (genes[r.GetInt(0, genes.Count)] as ICloneable).Clone());
                }
            }
        }
    }
}
