﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Alternating-position (AP).
    /// <remarks>
    /// <para>
    /// The alternating position crossover operator (Larrañaga et al. 1996a) simply creates an offspring by selecting alternately the next 
    /// element of the first parent and the next element of the second parent, omitting the elements already present in the offspring
    /// </para>
    /// <para>
    /// For example, if parent 1 is (1 2 3 4 5 6 7 8) and parent 2 is (3 7 5 1 6 8 2 4) 
    /// the AP operator gives the following offspring: (1 3 2 7 5 4 6 8)
    /// </para>
    /// <para>
    /// Exchanging the parents results in (3 1 7 2 5 4 6 8).
    /// <see href="../docs/Genetic Algorithms for the Travelling Salesman Problem - A Review of Representations and Operators.pdf">Genetic Algorithms for the Travelling Salesman Problem: A Review of Representations and Operators</see>
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Alternating-position (AP)")]
    public sealed class AlternatingPositionCrossover : CrossoverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.VotingRecombinationCrossover"/> class.
        /// </summary>
        public AlternatingPositionCrossover() : base()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            IsOrdered = true;
        }

        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();
            
            if (datas.AnyHasRepeatedValue())
            {
                throw new CrossoverException(this, "The Alternating-position (AP) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var p1 = datas[0];
            var p2 = datas[1];

            var child1 = parents[0].CreateNewChromosome();
            var child2 = parents[0].CreateNewChromosome();

            child1.SetDataSequence(CreateChildValues(p1,p2));
            child2.SetDataSequence(CreateChildValues(p2, p1));

            

            return new List<ChromosomeBase> { child1, child2 };
        }

        /// <summary>
        /// Creates an array of child values by combining the elements of two parent arrays.
        /// </summary>
        /// <param name="firstParent">The first parent array.</param>
        /// <param name="secondParent">The second parent array.</param>
        /// <returns>An array of objects representing the child values.</returns>
        private object[] CreateChildValues(object[] firstParent, object[] secondParent)
        {
            var childValues = new object[firstParent.Length];
            var childValuesIndex = 0;

            for (int i = 0; i < firstParent.Length && childValuesIndex < firstParent.Length; i++)
            {
                AddChildGene(childValues, ref childValuesIndex, firstParent[i]);

                // The childGenesIndes could be incremented by the previous AddChildGene call
                if (childValuesIndex < secondParent.Length)
                    AddChildGene(childValues, ref childValuesIndex, secondParent[i]);
            }

            return childValues;
        }

        /// <summary>
        /// Adds a gene to an array of child genes, if it is not already present in the array.
        /// </summary>
        /// <param name="childGenes">The array of child genes.</param>
        /// <param name="childGenesIndex">The current index in the child genes array.</param>
        /// <param name="parentGene">The gene to add to the child genes array.</param>
        private static void AddChildGene(object[] childGenes, ref int childGenesIndex, object parentGene)
        {
            if (!childGenes.Contains(parentGene))
            {
                childGenes[childGenesIndex] = parentGene;
                childGenesIndex++;
            }
        }
    }
}
