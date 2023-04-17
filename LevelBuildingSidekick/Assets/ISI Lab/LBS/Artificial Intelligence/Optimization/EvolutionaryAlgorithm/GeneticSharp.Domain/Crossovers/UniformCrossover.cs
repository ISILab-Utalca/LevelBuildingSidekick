using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// The Uniform Crossover uses a fixed mixing ratio between two parents. 
    /// Unlike one-point and two-point crossover, the Uniform Crossover enables the parent chromosomes to contribute the gene level rather than the segment level.
    /// <remarks>
    /// If the mix probability is 0.5, the offspring has approximately half of the genes from first parent and the other half from second parent, although cross over points can be randomly chosen.
    /// </remarks>
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Uniform_Crossover_and_Half_Uniform_Crossover">Wikipedia</see>
    /// </summary>
    [DisplayName("Uniform")]
    public class UniformCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.UniformCrossover"/> class.
        /// </summary>
        /// <param name="mixProbability">The mix probability. he default mix probability is 0.5.</param>
        public UniformCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            MixProbability = 0.5f;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mix probability.
        /// </summary>
        /// <value>The mix probability.</value>
        public float MixProbability { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();

            var firstParent = datas[0];
            var secondParent = datas[1];
            var firstChild = new object[firstParent.Length];
            var secondChild = new object[secondParent.Length];

            for (int i = 0; i < firstParent.Length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < MixProbability)
                {
                    firstChild[i] = firstParent[i];
                    secondChild[i] = secondParent[i];
                }
                else
                {
                    firstChild[i] = secondParent[i];
                    secondChild[i] = firstParent[i];
                }
            }

            var child1 = parents[0].CreateNewChromosome();
            var child2 = parents[0].CreateNewChromosome();

            return new List<ChromosomeBase> { child1, child2 };
        }
        #endregion
    }
}
