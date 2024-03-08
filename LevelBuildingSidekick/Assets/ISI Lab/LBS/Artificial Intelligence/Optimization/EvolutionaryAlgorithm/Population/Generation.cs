using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using System.Diagnostics;

namespace ISILab.AI.Optimization.Populations
{
    /// <summary>
    /// Represents a generation of a population.
    /// </summary>
    [DebuggerDisplay("{Number} = {BestChromosome.Fitness}")]
    public sealed class Generation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.Generation"/> class.
        /// </summary>
        /// <param name="number">The generation number.</param>
        /// <param name="evaluables">The chromosomes of the generation..</param>
        public Generation(int number, IList<IOptimizable> evaluables)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    "Generation number can not be lesser than 0. ".With(number));
            }

            if (evaluables == null)
            {
                throw new NullReferenceException("A generation can not be null.");
            }

            Number = number;
            CreationDate = DateTime.Now;
            Evaluables = evaluables;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Gets the chromosomes.
        /// </summary>
        /// <value>The chromosomes.</value>
        public IList<IOptimizable> Evaluables { get; internal set; }

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        public IOptimizable BestCandidate { get; internal set; }
        #endregion

        #region Methods
        /// <summary>
        /// Ends the generation.
        /// </summary>
        /// <param name="evaluablesCount">Evaluables number to keep on generation.</param>
        public void End(int evaluablesCount)
        {
            Evaluables = Evaluables
                .Where(ValidateEvaluables)
                .OrderByDescending(c => c.Fitness)
                .ToList();

            BestCandidate = Evaluables.First();

            if(evaluablesCount == 0)
            {
                throw new ArgumentException("Can not keep generations with size 0");
            }

            if(evaluablesCount < 0)
            {
                return;
            }

            if (Evaluables.Count > evaluablesCount)
            {
                Evaluables = Evaluables.Take(evaluablesCount).ToList();
            }

        }

        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to validate.</param>
        /// <returns>True if a chromosome is valid.</returns>
        private static bool ValidateEvaluables(IOptimizable chromosome)
        {
            if (chromosome.Fitness == float.NaN)
            {
                throw new InvalidOperationException("There is unknown problem in current generation, because a chromosome has no fitness value.");
            }

            return true;
        }
        #endregion
    }
}
