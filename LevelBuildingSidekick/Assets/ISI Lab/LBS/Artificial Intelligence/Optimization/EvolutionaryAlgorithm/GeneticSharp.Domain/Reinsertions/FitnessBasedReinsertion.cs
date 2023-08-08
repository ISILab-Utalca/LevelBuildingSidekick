using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using ISILab.AI.Optimization.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Fitness Based Reinsertion.
    /// <remarks>
    /// When there are more offspring than parents, select the only the best offspring to be reinserted, the parents are discarded.     
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Fitness Based")]
    public class FitnessBasedReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.FitnessBasedReinsertion"/> class.
        /// </summary>
        public FitnessBasedReinsertion() : base(true, false)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        protected override IList<IOptimizable> PerformSelectChromosomes(IPopulation population, IList<IOptimizable> offspring, IList<IOptimizable> parents)
        {
            if (offspring.Count > population.MaxSize)
            {
                return offspring.OrderByDescending(o => o.Fitness).Take(population.MaxSize).ToList();
            }

            return offspring;
        }
        #endregion
    }
}