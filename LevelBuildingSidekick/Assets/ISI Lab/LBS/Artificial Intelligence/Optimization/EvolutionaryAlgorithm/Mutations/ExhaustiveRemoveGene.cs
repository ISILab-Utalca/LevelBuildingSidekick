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
    public class ExhaustiveRemoveGene : MutationBase
    {
        protected override void PerformMutate(ChromosomeBase chromosome, float probability)
        {
            //Debug.Log("Performing REMOVE GENE Mutation");
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
                        chromosome.SetDeafult(i);
                    }
                }
            }
        }
    }
}