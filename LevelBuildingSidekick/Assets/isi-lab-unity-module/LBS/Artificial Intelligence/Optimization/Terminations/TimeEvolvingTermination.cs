using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization.Terminations
{
    /// <summary>
    /// Time Evolving Termination.
    /// <remarks>
    /// The genetic algorithm will be terminate when the evolving exceed the max time specified.
    /// </remarks>
    /// </summary>
    [DisplayName("Time Evolving")]
    public class TimeEvolvingTermination : TerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default MaxTime is 1 minute.
        /// </remarks>
        public TimeEvolvingTermination()
        { 
            MaxTime = TimeSpan.FromMinutes(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
        /// </summary>
        /// <param name="maxTime">The execution time to consider the termination has been reached.</param>
        public TimeEvolvingTermination(TimeSpan maxTime)
        {
            MaxTime = maxTime;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the execution max time.
        /// </summary>
        /// <value>The max time.</value>
        public TimeSpan MaxTime { get; set; }


        #endregion
        #region implemented abstract members of TerminationBase
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="optimizer">The genetic algorithm.</param>
        protected override bool PerformHasReached(BaseOptimizer optimizer)
        {
            return optimizer.TimeEvolving >= MaxTime;
        }
        #endregion
    }
}
