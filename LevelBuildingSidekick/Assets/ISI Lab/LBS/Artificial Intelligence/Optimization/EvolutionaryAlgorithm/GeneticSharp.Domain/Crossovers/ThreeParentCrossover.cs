using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using System.Linq;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Three Parent Crossover.
    /// <remarks>
    /// In this technique, the child is derived from three parents. 
    /// They are randomly chosen. Each bit of first parent is checked with bit of second parent whether they are same. 
    /// If same then the bit is taken for the offspring otherwise the bit from the third parent is taken for the offspring.
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Three_parent_crossover">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Three Parent")]
    public class ThreeParentCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeParentCrossover"/> class.
        /// </summary>
        public ThreeParentCrossover()
        {
            ParentsNumber = 3;
            ChildrenNumber = 1;
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
            var parent1Genes = datas;
            var parent2Genes = datas[1];
            var parent3Genes = datas[2];
            var offspring = new object[parent1.Length];
            object parent1Gene;

            for (int i = 0; i < parent1.Length; i++)
            {
                parent1Gene = parent1Genes[i];

                if (parent1Gene == parent2Genes[i])
                {
                    offspring[i] = parent1Gene;
                }
                else
                {
                    offspring[i] = parent3Genes[i];
                }
            }

            var child = parents[0].CreateNewChromosome();
            child.SetDataSequence(offspring);

            return new List<ChromosomeBase>() { child };
        }
        #endregion
    }
}
