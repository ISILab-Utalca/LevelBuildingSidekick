﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization.Terminations
{
    /// <summary>
    /// A base class for logical operator terminations.
    /// </summary>
    public abstract class LogicalOperatorTerminationBase : ITermination
    {
        #region Fields
        private readonly int m_minOperands;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperatorTerminationBase"/> class.
        /// </summary>
        /// <param name="minOperands">The minimum number of operands.</param>
        protected LogicalOperatorTerminationBase(int minOperands)
        {
            m_minOperands = minOperands;
            Terminations = new List<ITermination>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperatorTerminationBase"/> class.
        /// </summary>
        /// <param name="terminations">The terminations.</param>
        protected LogicalOperatorTerminationBase(params ITermination[] terminations)
            : this(2)
        {
            if (terminations != null)
            {
                for (int i = 0; i < terminations.Length; i++)
                {
                    AddTermination(terminations[i]);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the terminations.
        /// </summary>
        protected IList<ITermination> Terminations { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// Adds the termination.
        /// </summary>
        /// <param name="termination">The termination.</param>
        public void AddTermination(ITermination termination)
        {
            ExceptionHelper.ThrowIfNull("termination", termination);

            Terminations.Add(termination);
        }

        public VisualElement CIGUI()
        {
            return new Label("Not implemented");
        }

        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <param name="optimizer">The genetic algorithm.</param>
        /// <returns>
        /// True if termination has been reached, otherwise false.
        /// </returns>
        public bool HasReached(BaseOptimizer optimizer)
        {
            ExceptionHelper.ThrowIfNull("geneticAlgorithm", optimizer);

            if (Terminations.Count < m_minOperands)
            {
                throw new InvalidOperationException("The {0} needs at least {1} terminations to perform. Please, add the missing terminations.".With(GetType().Name, m_minOperands));
            }

            return PerformHasReached(optimizer);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.</returns>
        public override string ToString()
        {
            return "{0} ({1})".With(GetType().Name, String.Join(", ", Terminations.Select(t => t.ToString()).ToArray()));
        }

        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <param name="optimizer">The genetic algorithm.</param>
        /// <returns>
        /// True if termination has been reached, otherwise false.
        /// </returns>
        protected abstract bool PerformHasReached(BaseOptimizer optimizer);
        #endregion
    }
}
