using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Cut and Splice crossover.
    /// <remarks>
    /// Results in a change in length of the children strings. The reason for this difference is that each parent string has a separate choice of crossover point.
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#.22Cut_and_splice.22">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Cut and Splice")]
    public class CutAndSpliceCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CutAndSpliceCrossover"/> class.
        /// </summary>
        public CutAndSpliceCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            IsOrdered = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();

            var parent1 = datas[0];
            var parent2 = datas[1];

            // The minium swap point is 1 to safe generate a gene with at least two genes.
            var parent1Point = RandomizationProvider.Current.GetInt(1, parent1.Length) + 1;
            var parent2Point = RandomizationProvider.Current.GetInt(1, parent2.Length) + 1;

            var offspring1 = parents[0].CreateNewChromosome();
            var offspring2 = parents[0].CreateNewChromosome();

            offspring1.SetDataSequence(CreateOffspring(parent1, parent2, parent1Point, parent2Point));
            offspring2.SetDataSequence(CreateOffspring(parent2, parent1, parent2Point, parent1Point));

            return new List<ChromosomeBase>() { offspring1, offspring2 };
        }

        private static object[] CreateOffspring(object[] leftParent, object[] rightParent, int leftParentPoint, int rightParentPoint)
        {
            var offspring = new object[leftParentPoint + (rightParent.Length - rightParentPoint)];
            var left = leftParent.Take(leftParentPoint).ToArray();
            var right = rightParent.Skip(rightParentPoint).ToArray();
            Array.Copy(left, 0, offspring, 0, left.Length);
            Array.Copy(right,0,offspring,leftParentPoint,right.Length);

            return offspring;
        }
        #endregion
    }
}
