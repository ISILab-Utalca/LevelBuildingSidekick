using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExhaustiveSwapGene : MutationBase
{ 

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
                    var j = r.GetInt(0, chromosome.Length);
                    if (chromosome.IsImmutable(j))
                        continue;

                    var aux = chromosome.GetGene(i);
                    chromosome.ReplaceGene(i, chromosome.GetGene(j));
                    chromosome.ReplaceGene(j, aux);
                }
            }
        }
    }
}
