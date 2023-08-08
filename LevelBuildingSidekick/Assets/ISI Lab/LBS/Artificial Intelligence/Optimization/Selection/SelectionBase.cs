using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using ISILab.AI.Optimization.Populations;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using System.Linq;
using UnityEngine.UIElements;
using Commons.Optimization.Evaluator;

namespace ISILab.AI.Optimization.Selections
{
    /// <summary>
    /// A base class for selection.
    /// </summary>
    public abstract class SelectionBase : ISelection
    {
        #region Fields
        public int m_minNumberChromosomes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.SelectionBase"/> class.
        /// </summary>
        /// <param name="minNumberChromosomes">Minimum number chromosomes support to be selected.</param>
        protected SelectionBase(int minNumberChromosomes)
        {
            m_minNumberChromosomes = minNumberChromosomes;
        }

        public SelectionBase()
        {
            m_minNumberChromosomes = 2;
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Selects the number of chromosomes from the generation specified.
        /// </summary>
        /// <returns>The selected chromosomes.</returns>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        public IList<IOptimizable> SelectEvaluables(int number, Generation generation)
        {

            ExceptionHelper.ThrowIfNull("generation", generation);

            if (generation.Evaluables.Any(c => c.Fitness == double.NaN))
            {
                throw new SelectionException(
                       this,
                       "There are chromosomes with null fitness.");
            }

            return PerformSelectEvaluables(number, generation);
        }

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <returns>The selected chromosomes.</returns>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        protected abstract IList<IOptimizable> PerformSelectEvaluables(int number, Generation generation);

        public List<IOptimizable> GetBetters(IEvaluator evaluator, double? score)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}