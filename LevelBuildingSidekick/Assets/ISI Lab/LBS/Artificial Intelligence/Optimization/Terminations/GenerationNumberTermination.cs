using System.ComponentModel;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization.Terminations
{
    /// <summary>
    /// Generation number termination.
    /// <remarks>
    /// The genetic algorithm will be terminate when reach the expected generation number.
    /// </remarks>
    /// </summary>
    [DisplayName("Generation Number")]
    public class GenerationNumberTermination : TerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default expected generation number is 100.
        /// </remarks>
        public GenerationNumberTermination()
        {
            ExpectedGenerationNumber = 500;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
        /// </summary>
        /// <param name="expectedGenerationNumber">The generation number to consider the termination has been reached.</param>
        public GenerationNumberTermination(int expectedGenerationNumber)
        {
            ExpectedGenerationNumber = expectedGenerationNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the expected generation number to consider that termination has been reached.
        /// </summary>
        /// <value>The generation number.</value>
        public int ExpectedGenerationNumber { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="optimizer">The genetic algorithm.</param>
        protected override bool PerformHasReached(BaseOptimizer optimizer)
        {
            return optimizer.GenerationsNumber >= ExpectedGenerationNumber;
        }
        #endregion
    }
}