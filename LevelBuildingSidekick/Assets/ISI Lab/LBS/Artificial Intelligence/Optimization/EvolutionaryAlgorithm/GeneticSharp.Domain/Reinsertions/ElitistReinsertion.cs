using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using ISILab.AI.Optimization.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Elitist reinsertion.
    /// <remarks>
    /// When there are less offspring than parents, select the best parents to be reinserted together with the offspring. 
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Elitist")]
    public class ElitistReinsertion : ReinsertionBase
    {
        int ParentReinsertions = 1;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ElitistReinsertion"/> class.
        /// </summary>
        public ElitistReinsertion() : base(false, true)
        {
        }

        public ElitistReinsertion(int parentReinsertions) : base(false, true)
        {
            ParentReinsertions = parentReinsertions;
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
            var aux = parents.Distinct().OrderByDescending(p => p.Fitness).ToList();
            var bestParents = offspring.OrderByDescending(p => p.Fitness).ToList();

            for(int i = 0; i < ParentReinsertions; i++)
            {
                if (aux.Count == 0)
                    break;
                if(bestParents.Count < population.MaxSize)
                {
                    bestParents.Insert(0, aux[0]);
                    aux.RemoveAt(0);
                }

                if(bestParents[^1].Fitness < aux[0].Fitness)
                {
                    bestParents.RemoveAt(bestParents.Count - 1);
                    bestParents.Insert(0, aux[0]);
                    aux.RemoveAt(0);
                }
                else
                    break;
            }

            var count = population.MaxSize < bestParents.Count ? population.MaxSize : bestParents.Count;

            return bestParents.Take(count).ToList();
        }
        #endregion
    }
}