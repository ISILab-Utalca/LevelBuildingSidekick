using Commons.Optimization.Terminations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOptimizer
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
    IEvaluable BestCandidate { get; }

    /// <summary>
    /// Gets the time evolving.
    /// </summary>
    /// <value>The time evolving.</value>
    TimeSpan TimeEvolving { get; }

    public ITermination Termination { get; set; }

    public bool IsRunning { get; }
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
