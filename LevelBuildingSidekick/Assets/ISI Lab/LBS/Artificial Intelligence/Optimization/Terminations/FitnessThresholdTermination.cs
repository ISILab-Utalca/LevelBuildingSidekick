using System.ComponentModel;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization.Terminations
{
    /// <summary>
    /// Fitness Threshold Termination
    /// <remarks>
    /// The genetic algorithm will be terminate when the best chromosome reach the expected fitness.
    /// </remarks>
    /// </summary>
    [DisplayName("Fitness Threshold")]
    public class FitnessThresholdTermination : TerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.FitnessThresholdTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default expected fitness is 1.00.
        /// </remarks>
        public FitnessThresholdTermination()
        {
            ExpectedFitness = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.FitnessThresholdTermination"/> class.
        /// </summary>
        /// <param name="expectedFitness">Expected fitness.</param>
        public FitnessThresholdTermination(double expectedFitness)
        {
            ExpectedFitness = expectedFitness;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the expected fitness to consider that termination has been reached.
        /// </summary>
        public double ExpectedFitness { get; set; }
        #endregion

        #region implemented abstract members of TerminationBase
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="optimizer">The genetic algorithm.</param>
        protected override bool PerformHasReached(BaseOptimizer optimizer)
        {
            return optimizer.BestCandidate.Fitness >= ExpectedFitness;
        }
        #endregion
    }
}
