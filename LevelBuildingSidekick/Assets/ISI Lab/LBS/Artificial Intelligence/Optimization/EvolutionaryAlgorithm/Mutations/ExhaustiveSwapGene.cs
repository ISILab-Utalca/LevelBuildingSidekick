using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using LBS.Components.TileMap;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    [System.Serializable]
    public class ExhaustiveSwapGene : MutationBase
    {
        protected override void PerformMutate(ChromosomeBase chromosome, float probability)
        {
            //Debug.Log("Performing SWAP GENE Mutation");
            var r = RandomizationProvider.Current;

            for (int i = 0; i < chromosome.Length; i++)
            {
                if (chromosome.IsImmutable(i) || chromosome.IsInvalid(i))
                    continue;
                if (chromosome.GetGene(i) != default)
                {
                    var d = r.GetDouble();
                    if (d < probability)
                    {
                        var j = r.GetInt(0, chromosome.Length);
                        if (chromosome.IsImmutable(j) || chromosome.IsInvalid(j))
                            continue;

                        var aux = chromosome.GetGene(i);
                        chromosome.ReplaceGene(i, chromosome.GetGene(j));
                        chromosome.ReplaceGene(j, aux);
                    }
                }
            }
        }
    }
}