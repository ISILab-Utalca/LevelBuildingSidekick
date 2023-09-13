using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExhaustiveRemoveGene : MutationBase
{
    [SerializeField, SerializeReference]
    public List<object> blackList = new List<object>();

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;


        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome.IsImmutable(i))
                continue;
            if (chromosome.GetGene(i) != default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    if(!blackList.Contains(chromosome.GetGene(i)))
                        chromosome.SetDeafult(i);
                }
            }
        }
    }
}
