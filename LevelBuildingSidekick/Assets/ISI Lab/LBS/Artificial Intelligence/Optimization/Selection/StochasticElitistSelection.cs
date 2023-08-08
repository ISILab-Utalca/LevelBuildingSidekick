using ISILab.AI.Optimization.Populations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization.Selections
{

    public class StochasticElitistSelection : SelectionBase
    {
        double last = 0;
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.EliteSelection"/> class.
        /// </summary>
        public StochasticElitistSelection() : base()
        {
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IOptimizable> PerformSelectEvaluables(int number, Generation generation)
        {
            var r = RandomizationProvider.Current;
            var ordered = generation.Evaluables.Where(c => c.Fitness >= last).OrderBy(c => r.GetDouble());
            last = ordered.First().Fitness;
            return ordered.Take(number).ToList();   
        }

        #endregion
    }
}
