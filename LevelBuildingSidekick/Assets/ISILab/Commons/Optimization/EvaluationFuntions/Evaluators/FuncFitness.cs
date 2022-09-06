using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace Commons.Optimization.Fitness
{
    /// <summary>
    /// An IFitness implementation that defer the fitness evaluation to a Func.
    /// </summary>
    public class FuncFitness : IEvaluator
    {
        private readonly Func<IEvaluable, float> m_func;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Fitnesses.FuncFitness"/> class.
        /// </summary>
        /// <param name="func">The fitness evaluation Func.</param>
        public FuncFitness (Func<IEvaluable, float> func)
        {
            ExceptionHelper.ThrowIfNull("func", func);
            m_func = func;
        }

        #region IFitness implementation
        /// <summary>
        /// Evaluate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">Chromosome.</param>
        public float Evaluate (IEvaluable chromosome)
        {
            return m_func (chromosome);
        }
        #endregion
    }
}