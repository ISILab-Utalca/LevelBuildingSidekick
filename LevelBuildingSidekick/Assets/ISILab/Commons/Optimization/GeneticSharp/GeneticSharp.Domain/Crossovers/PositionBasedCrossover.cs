﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Position-based crossover (POS).
    /// <remarks>
    /// The position-based crossover operator (POS), which was also suggested in connection with schedule problems, 
    /// is a second modification of the OX1 operator. It also starts with selecting a random set of positions in 
    /// the parent strings. However, this operator imposes the position of the selected elements on the 
    /// corresponding elements of the other parent. For example, consider the parents (1 2 3 4 5 6 7 8) 
    /// and (2 4 6 8 7 5 3 1), and suppose that the second, third and sixth positions are selected. 
    /// This leads to the following offspring: (1 4 6 2 3 5 7 8) and (4 2 3 8 7 6 5 1).
    /// </remarks>
    /// </summary>
    [DisplayName("Position-based (POS)")]
    public class PositionBasedCrossover : OrderBasedCrossover
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.PositionBasedCrossover"/> class.
        /// </summary>
        public PositionBasedCrossover()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates the parents.
        /// </summary>
        /// <param name="parents">The parents.</param>
        protected override void ValidateParents(IList<object[]> parents)
        {
            if (parents.AnyHasRepeatedValue())
            {
                throw new CrossoverException(this, "The Position-based Crossover (POS) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="swapIndexes">The swap indexes.</param>
        /// <returns>
        /// The child.
        /// </returns>
        protected override object[] CreateChild(object[] firstParent, object[] secondParent, int[] swapIndexes)
        {
            var secondParentSwapGenes = secondParent
                 .Select((g, i) => new { Gene = g, Index = i })
                 .Where((g) => swapIndexes.Contains(g.Index))
                 .ToArray();

            using (var firstParentRemainingGenes = firstParent
                .Except(secondParentSwapGenes.Select(element => element.Gene).ToArray()).GetEnumerator())
            {
                var child = new object[firstParent.Length];
                var secondParentSwapGensIndex = 0;

                for (int i = 0; i < firstParent.Length; i++)
                {
                    if (secondParentSwapGenes.Any(f => f.Index == i))
                    {
                        child[i] = secondParentSwapGenes[secondParentSwapGensIndex++];
                    }
                    else
                    {
                        firstParentRemainingGenes.MoveNext();
                        child[i] = firstParentRemainingGenes.Current;
                    }
                }

                return child;
            }
        }

        #endregion
    }
}
