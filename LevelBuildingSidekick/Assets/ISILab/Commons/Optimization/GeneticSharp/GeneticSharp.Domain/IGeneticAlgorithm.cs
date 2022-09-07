using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain
{
    /// <summary>
    /// Defines a interface for a genetic algorithm.
    /// </summary>
    public interface IGeneticAlgorithm : IOptimizer
    {
        #region Properties

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        IChromosome BestChromosome { get; }
        #endregion
    }
}
