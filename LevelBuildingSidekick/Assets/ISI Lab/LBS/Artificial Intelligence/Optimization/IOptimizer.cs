using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using System;
using GeneticSharp.Domain.Populations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Optimization
{ 
    public interface IOptimizer: INameable
    {
        #region Properties
        /// <summary>
        /// Gets the generations number.
        /// </summary>
        /// <value>The generations number.</value>
        int GenerationsNumber { get; }

        /// <summary>
        /// Gets the best candidate.
        /// </summary>
        /// <value>The best candidate.</value>
        IOptimizable BestCandidate { get; }
        IOptimizable Adam { get; set; }

        /// <summary>
        /// Gets the time evolving.
        /// </summary>
        /// <value>The time evolving.</value>
        TimeSpan TimeEvolving { get; }

        /// <summary>
        /// Gets the population.
        /// </summary>
        /// <value>The population.</value>
        public IPopulation Population { get; }

        public ITermination Termination { get; set; }
        public IEvaluator Evaluator { get;}

        public bool IsRunning { get; }
        public OptimizerState State { get; }
        public IOptimizable[] LastGeneration { get; }
        #endregion

        #region Events
        public Action OnGenerationRan { get; set; }
        public Action OnTerminationReached { get; set; }
        public Action OnStopped { get; set; }
        public Action OnResumed { get; set; }
        public Action OnPaused { get; set; }
        public Action OnStarted { get; set; }
        #endregion

        #region Methods
        public void Start();
        public void Resume();
        public void Pause();
        public void Stop();
        #endregion
    }

    public enum OptimizerState
    {
        /// <summary>
        /// The Optimizer has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The Optimizer has been started and is running.
        /// </summary>
        Started,

        /// <summary>
        /// The Optimizer has been stopped and is not running.
        /// </summary>
        Stopped,

        /// <summary>
        /// The Optimizer has been resumed after a stop or termination reach and is running.
        /// </summary>
        Resumed,

        /// <summary>
        /// The Optimizer has not been stopped or reached termination and is still running.
        /// </summary>
        Running,

        /// <summary>
        /// The Optimizer has reach the termination condition and is not running.
        /// </summary>
        TerminationReached,

        /// <summary>
        /// The Optimizer has been paused and is not running.
        /// </summary>
        Paused
    }
}