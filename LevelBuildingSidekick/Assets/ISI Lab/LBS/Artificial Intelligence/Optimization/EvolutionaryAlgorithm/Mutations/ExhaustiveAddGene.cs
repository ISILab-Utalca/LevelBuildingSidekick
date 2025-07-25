using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using LBS.Components.TileMap;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    [System.Serializable]
    public class ExhaustiveAddGene : MutationBase
    {
        protected override void PerformMutate(ChromosomeBase chromosome, float probability)
        {
            //Debug.Log("Performing ADD GENE Mutation");
            var bc = chromosome as BundleTilemapChromosome;

            var r = RandomizationProvider.Current;
            var mutables = bc.GetGenes().Select((g, i) => new { g, i }).Where(x => x.g != null && !(chromosome.IsImmutable(x.i) || chromosome.IsInvalid(x.i)));
            var genes = mutables.Select(x => x.g).Distinct().Cast<BundleData>().ToList();

            if (genes.Count == 0)
                return;

            for (int i = 0; i < chromosome.Length; i++)
            {
                if (chromosome.IsImmutable(i) || chromosome.IsInvalid(i))
                    continue;
                if (chromosome.GetGene(i) == null)
                {
                    var d = r.GetDouble();
                    if (d < probability)
                    {
                        var index = r.GetInt(0, genes.Count);
                        var gen = genes[index];
                        chromosome.ReplaceGene(i, (gen as ICloneable).Clone());
                    }
                }
            }
        }
    }
}
