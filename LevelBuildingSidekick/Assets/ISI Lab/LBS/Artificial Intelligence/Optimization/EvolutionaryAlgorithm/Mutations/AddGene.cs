using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    public class AddGene : MutationBase
    {
        public List<object> blackList = new List<object>();

        protected override void PerformMutate(ChromosomeBase chromosome, float probability)
        {
            var r = RandomizationProvider.Current;

            var d = r.GetDouble();

            if (d > probability)
            {
                return;
            }

            var i = r.GetInt(0, chromosome.Length);

            while (i < chromosome.Length && (chromosome.GetGene(i) != default || blackList.Contains(chromosome.GetGene(i))))
            {
                i++;
            }

            if (i < chromosome.Length)
            {
                chromosome.ReplaceGene(i, chromosome.GenerateGene());
            }
        }
    }
}