using System;
using System.Collections.Generic;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Populations;

namespace ISILab.AI.Optimization.Selections
{
    /// <summary>
    /// Defines a interface for selection.
    /// Selection is the stage of a genetic algorithm in which individual genomes are chosen from a population for later breeding (recombination or crossover).
    /// <see href="http://en.wikipedia.org/wiki/Selection_(genetic_algorithm)">Wikipedia</see>
    /// <see href=" http://www.ijest.info/docs/IJEST11-03-05-190.pdf">A Review of Selection Methods in Genetic Algorithm</see>
    /// </summary>
    public interface ISelection //: IShowable
    {
        /// <summary>
        /// Selects the number of chromosomes from the generation specified.
        /// </summary>
        /// <returns>The selected chromosomes.</returns>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        IList<IOptimizable> SelectEvaluables(int number, Generation generation);
        List<IOptimizable> GetBetters(IEvaluator evaluator, double? score);
    }
}