﻿using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace ISILab.AI.Optimization.Populations
{
    /// <summary>
    /// Defines an interface for a population of candidate solutions (chromosomes).
    /// </summary>
    public interface IPopulation //: IShowable
    {
        #region Events
        /// <summary>
        /// Occurs when best chromosome changed.
        /// </summary>
        public Action OnBestcandidateChanged { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the creation date.
        /// </summary>
        DateTime CreationDate { get; }

        /// <summary>
        /// Gets the generations.
        /// <remarks>
        /// The information of Generations can vary depending of the IGenerationStrategy used.
        /// </remarks>
        /// </summary>
        /// <value>The generations.</value>
        IList<Generation> Generations { get; }

        /// <summary>
        /// Gets the current generation.
        /// </summary>
        /// <value>The current generation.</value>
        Generation CurrentGeneration { get; }

        /// <summary>
        /// Gets the total number of generations executed.
        /// <remarks>
        /// Use this information to know how many generations have been executed, because Generations.Count can vary depending of the IGenerationStrategy used.
        /// </remarks>
        /// </summary>
        int GenerationsNumber { get; }

        /// <summary>
        /// Gets or sets the minimum size.
        /// </summary>
        /// <value>The minimum size.</value>
        int MinSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the max.
        /// </summary>
        /// <value>The size of the max.</value>
        int MaxSize { get; set; }

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        IOptimizable BestCandidate { get; }

        /// <summary>
        /// Gets or sets the generation strategy.
        /// </summary>
        IGenerationStrategy GenerationStrategy { get; set; }
        public IOptimizable Adam { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the initial generation.
        /// </summary>
        void CreateInitialGeneration(int GenSize = 0);

        /// <summary>
        /// Creates a new generation.
        /// </summary>
        /// <param name="chromosomes">The chromosomes for new generation.</param>
        void CreateNewGeneration(IList<IOptimizable> chromosomes);

        /// <summary>
        /// Ends the current generation.
        /// </summary>        
        void EndCurrentGeneration();
        #endregion
    }
}
