﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Voting Recombination Crossover (VR).
    /// <remarks>
    /// <para>
    /// It can be seen as a P-sexual crossover operator, where p (parents number) is a natural number greater than, or equal to, 2.
    /// </para>
    /// <para>
    /// It starts by defining a threshold, which is a natural number smaller than, or equal to p.
    /// </para>
    /// <para>
    /// Next, for every; i E {l, 2, . . .N} the set of ith elements of all the parents is considered. 
    /// If in this set an element occurs at least the threshold number of times, it is copied into the offspring. 
    /// </para>
    /// <para>
    /// For example, if we consider the parents(p = 4) (1 4 3 5 2 6), (1 2 4 3 5 6), (3 2 1 5 4 6), (1 2 3 4 5 6) and we define the threshold to be equal to 3 we find(1 2 x x x 6). 
    /// The remaining positions of the offspring are filled with mutations.
    /// <see href="http://ictactjournals.in/paper/IJSC_V6_I1_paper_4_pp_1083_1092.pdf">Crossover operators in genetic algorithms: A review</see>
    /// </para>
    /// <para>
    /// The voting recombination produce one child of parents number (p) based on a threshold.
    /// <see href="https://www.cs.vu.nl/~gusz/papers/Handbook-Multiparent-Eiben.pdf">Multiparent Recombination</see>
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Voting Recombination (VR)")]
    public sealed class VotingRecombinationCrossover : CrossoverBase
    {
        private int _threshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.VotingRecombinationCrossover"/> class.
        /// </summary>
        /// <param name="parentsNumber">The number of parents need for cross.</param>
        /// <param name="threshold">An element occurs at least the threshold number of times, it is copied into the offspring</param>
        public VotingRecombinationCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 1;
            _threshold = 2;
            IsOrdered = false;
        }

        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();

            var firstParent = datas[0];
            var data = new object[firstParent.Length];
            var mutableGenesIndexes = new List<int>();
           
            for (int i = 0; i < data.Length; i++)
            {
                // If in this set an element occurs at least the threshold number of times, it is copied into the offspring.
                 var moreOcurrencesGeneValue = datas
                                            .GroupBy(p => p[i])
                                            .Where(p => p.Count() >= _threshold)
                                            .OrderByDescending(g => g.Count())
                                            .FirstOrDefault();

                if (moreOcurrencesGeneValue != null)
                {
                    data[i] = moreOcurrencesGeneValue.Key;
                }
                else
                {
                    mutableGenesIndexes.Add(i);
                }
            }

            var child = parents[0].CreateNewChromosome();
            child.SetDataSequence(data);

            // The remaining positions of the offspring are filled with mutations.
            if (mutableGenesIndexes.Count > 0)
            {
                new UniformMutation(mutableGenesIndexes.ToArray())
                    .Mutate(child, 1);
            }

            return new List<ChromosomeBase> { child };
        }
    }
}
