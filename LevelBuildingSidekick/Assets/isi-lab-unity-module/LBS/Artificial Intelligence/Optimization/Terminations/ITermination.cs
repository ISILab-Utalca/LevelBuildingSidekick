
namespace ISILab.AI.Optimization.Terminations
{
    /// <summary>
    /// Defines the interface for a termination condition.
    /// </summary>
    /// <remarks>
    /// <see href="http://en.wikipedia.org/wiki/Genetic_algorithm#Termination">Wikipedia</see> 
    /// </remarks>
    public interface ITermination //: IShowable
    {
        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        bool HasReached(BaseOptimizer optimizer);
        #endregion
    }
}
