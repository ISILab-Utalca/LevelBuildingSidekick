
using System;

namespace Commons.Optimization.Evaluator
{
    /// <summary>
    /// Defines an interface for fitness function.
    /// <remarks>
    /// A fitness function is a particular type of objective function that is used to summarize, as a single figure of merit, how close a given design solution is to achieving the set aims.
    /// <see href="http://en.wikipedia.org/wiki/Fitness_function">Wikipedia</see>
    /// </remarks>
    /// </summary>
    public interface IEvaluator : ICloneable//: INameable//, IShowable
    {
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        float Evaluate(IOptimizable evaluable);
    }

    public interface INameable
    {
        string GetName();
    }
}
