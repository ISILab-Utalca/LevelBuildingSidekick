using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ExhaustiveAddGene : MutationBase
{
    [SerializeField, SerializeReference]
    public List<object> blackList = new List<object>();

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;
        var genes = chromosome.GetGenes().Where(g => g != null).Distinct().ToList(); //Distinct is not doing anything

        if (genes.Count == 0)
            return;

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome.IsImmutable(i))
                continue;
            if(chromosome.GetGene(i) == default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    var index = r.GetInt(0, genes.Count);
                    var gen = (genes[index] as ICloneable).Clone();

                    if (!blackList.Contains(gen))
                    {
                        chromosome.ReplaceGene(i, gen);
                    }
                }
            }
        }
    }
}
