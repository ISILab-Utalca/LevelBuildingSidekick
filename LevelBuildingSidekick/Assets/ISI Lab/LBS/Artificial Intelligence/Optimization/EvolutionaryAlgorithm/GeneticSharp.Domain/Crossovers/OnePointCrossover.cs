﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Texts;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// One-Point crossover (C1).
    /// <remarks>
    /// A single crossover point on both parents is selected. 
    /// All data beyond that point in either is swapped between the two parents.    
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#One-point_crossover">One-point crossover</see>
    /// <example>
    /// Parents: 
    /// |0|0|0| x |1|1|1|
    /// Have two swap points indexes: 0 and 1.
    /// <para>
    /// 1) 
    /// new OnePointCrossover(0);
    /// Children result:
    /// |0|1|1| and |1|0|0|
    /// </para>
    /// <para>
    /// 2) 
    /// new OnePointCrossover(1);
    /// Children result:
    /// |0|0|1| and |1|1|0|
    /// </para>
    /// </example>
    /// </remarks>
    /// </summary>
    [DisplayName("One-Point")]
    public class OnePointCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.OnePointCrossover"/> class.
        /// </summary>
        /// <param name="swapPointIndex">Swap point index.</param>
        public OnePointCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            SwapPointIndex = 0;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the index of the swap point.
        /// </summary>
        /// <value>The index of the swap point.</value>
        public int SwapPointIndex { get; set; }
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

            var firstParent = datas[0];
            var secondParent = datas[1];

            var swapPointsLength = firstParent.Length - 1;

            if (SwapPointIndex >= swapPointsLength)
            {
                throw new ArgumentOutOfRangeException(
                    "parents",
                    "The swap point index is {0}, but there is only {1} genes. The swap should result at least one gene to each side.".With(SwapPointIndex, firstParent.Length));
            }

            List<ChromosomeBase> children = new List<ChromosomeBase>();
            var offspring = CreateChildren(firstParent, secondParent);

            foreach(var data in offspring)
            {
                var child = parents[0].CreateNewChromosome();
                child.SetDataSequence(data);
                children.Add(child);
            }

            return children;
        }

        /// <summary>
        /// Creates the children.
        /// </summary>
        /// <param name="firstParent">The first parent.</param>
        /// <param name="secondParent">The second parent.</param>
        /// <returns>The children chromosomes.</returns>
        protected IList<object[]> CreateChildren(object[] firstParent, object[] secondParent)
        {
            var firstChild = CreateChild(firstParent, secondParent);
            var secondChild = CreateChild(secondParent, firstParent);

            return new List<object[]>() { firstChild, secondChild };
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="leftParent">Left parent.</param>
        /// <param name="rightParent">Right parent.</param>
        protected virtual object[] CreateChild(object[] leftParent, object[] rightParent)
        {
            var cutGenesCount = SwapPointIndex + 1;
            var child = new object[leftParent.Length];

            var leftpart = leftParent.Take(cutGenesCount).ToArray();
            var rightPart = rightParent.Skip(cutGenesCount).ToArray();

            Array.Copy(leftpart, 0, child, 0, leftpart.Length);
            Array.Copy(rightPart, 0, child, cutGenesCount, rightPart.Length);

            return child;
        }
        #endregion
    }
}
