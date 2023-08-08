using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace ISILab.AI.Optimization.Populations
{
    /// <summary>
    /// Represents a population of candidate solutions (chromosomes).
    /// </summary>
    public class Population : IPopulation
    {

        #region Constructors
        public Population()
        {
            GenerationsNumber = 0;
            CreationDate = DateTime.Now;
            Generations = new List<Generation>();
            GenerationStrategy = new PerformanceGenerationStrategy(10);
            MinSize = 10;
            MaxSize = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.Population"/> class.
        /// </summary>
        /// <param name="minSize">The minimum size (chromosomes).</param>
        /// <param name="maxSize">The maximum size (chromosomes).</param>
        /// <param name="adam">The original chromosome of all population ;).</param>
        public Population(int minSize, int maxSize, IOptimizable adam)
        {
            GenerationsNumber = 0;

            if (maxSize < minSize)
            {
                throw new ArgumentOutOfRangeException("maxSize", "The maximum size for a population should be equal or greater than minimum size.");
            }

            ExceptionHelper.ThrowIfNull("adamChromosome", adam);

            CreationDate = DateTime.Now;
            MinSize = minSize;
            MaxSize = maxSize;
            Adam = adam;
            Generations = new List<Generation>();
            GenerationStrategy = new PerformanceGenerationStrategy(10);
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when best evaluable changed.
        /// </summary>
        public Action OnBestcandidateChanged { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Gets or sets the generations.
        /// <remarks>
        /// The information of Generations can vary depending of the IGenerationStrategy used.
        /// </remarks>
        /// </summary>
        /// <value>The generations.</value>
        public IList<Generation> Generations { get; protected set; }

        /// <summary>
        /// Gets or sets the current generation.
        /// </summary>
        /// <value>The current generation.</value>
        public Generation CurrentGeneration { get; protected set; }

        /// <summary>
        /// Gets or sets the total number of generations executed.
        /// <remarks>
        /// Use this information to know how many generations have been executed, because Generations.Count can vary depending of the IGenerationStrategy used.
        /// </remarks>
        /// </summary>
        public int GenerationsNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum size.
        /// </summary>
        /// <value>The minimum size.</value>
        public int MinSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the max.
        /// </summary>
        /// <value>The size of the max.</value>
        public int MaxSize { get; set; }

        /// <summary>
        /// Gets or sets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        public IOptimizable BestCandidate => Generations.Select(g => g.BestCandidate).OrderByDescending(b => b.Fitness).First();

        /// <summary>
        /// Gets or sets the generation strategy.
        /// </summary>
        public IGenerationStrategy GenerationStrategy { get; set; }

        /// <summary>
        /// Gets or sets the original chromosome of all population.
        /// </summary>
        /// <value>The adam chromosome.</value>
        public IOptimizable Adam { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Creates the initial generation.
        /// </summary>
        public virtual void CreateInitialGeneration()
        {
            Generations = new List<Generation>();
            GenerationsNumber = 0;

            var evaluables = new List<IOptimizable>();
            evaluables.Add(Adam);

            for (int i = 1; i < MinSize; i++)
            {
                var c = Adam.CreateNew();

                if (c == null)
                {
                    throw new InvalidOperationException("The Adam evaluable 'CreateNew' method generated a null evaluable. This is a invalid behavior, please, check your evaluable code.");
                }

                evaluables.Add(c);
            }

            CreateNewGeneration(evaluables);
        }

        /// <summary>
        /// Creates a new generation.
        /// </summary>
        /// <param name="optimizables">The chromosomes for new generation.</param>
        public virtual void CreateNewGeneration(IList<IOptimizable> optimizables)
        {
            ExceptionHelper.ThrowIfNull("chromosomes", optimizables);

            CurrentGeneration = new Generation(++GenerationsNumber, optimizables);
            Generations.Add(CurrentGeneration);
            GenerationStrategy.RegisterNewGeneration(this);
        }

        /// <summary>
        /// Ends the current generation.
        /// </summary>        
        public virtual void EndCurrentGeneration()
        {
            CurrentGeneration.End(MaxSize);
        }

        public VisualElement CIGUI()
        {
            var content = new VisualElement();

            var minSize = new IntegerField("Min Size");
            minSize.value = MinSize;
            minSize.RegisterValueChangedCallback(e => {
                MinSize = e.newValue;
            });
            var maxSize = new IntegerField("Min Size");
            maxSize.value = MaxSize;
            maxSize.RegisterValueChangedCallback(e => {
                MaxSize = e.newValue;
            });


            content.Add(minSize);
            content.Add(maxSize);

            return content;
        }
        #endregion
    }
}