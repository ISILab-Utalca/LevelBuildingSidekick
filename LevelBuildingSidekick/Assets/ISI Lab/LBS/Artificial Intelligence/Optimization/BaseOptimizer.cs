using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using GeneticSharp.Domain.Populations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using Commons.Optimization;

public abstract class BaseOptimizer : IOptimizer
{

    #region Properties
    public int GenerationsNumber { get; private set; }
    public bool IsRunning { get { return State == OptimizerState.Running || State == OptimizerState.Started || State == OptimizerState.Resumed; } }
    public TimeSpan TimeEvolving { get; private set; }

    public IEvaluable Adam { get; set; }
    public IEvaluable BestCandidate { get; private set; }

    /// <summary>
    /// Gets the population.
    /// </summary>
    /// <value>The population.</value>
    public IPopulation Population { get; private set; }
    public IEvaluable[] LastGeneration
    {
        get
        {
            return Population.CurrentGeneration.Evaluables.ToArray();
        }
    }
    public ITermination Termination { get; set; }
    public IEvaluator Evaluator { get; private set; }
    public OptimizerState State { 
        get
        {
            return state;
        }
        set
        {
            var shouldStop = OnStopped != null && state != value && value == OptimizerState.Stopped;

            state = value;

            if (shouldStop)
            {
                Stop();
            }
        }
    }
    #endregion

    #region Fields
    private OptimizerState state;
    private Stopwatch clock;
    private readonly object m_lock;
    private bool stopRequested;
    private bool pauseRequested;
    #endregion

    #region Events
    public Action OnGenerationRan { get; set; }
    public Action OnTerminationReached { get; set; }
    public Action OnStopped { get; set; }
    public Action OnResumed { get; set; }
    public Action OnPaused { get; set; }
    public Action OnStarted { get; set; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseOptimizer"/> class with the specified starting evaluable, evaluator, and termination condition.
    /// </summary>
    /// <param name="adam">The starting evaluable.</param>
    /// <param name="evaluator">The evaluator to use.</param>
    /// <param name="termination">The termination condition to use.</param>
    public BaseOptimizer(IEvaluable adam, IEvaluator evaluator, ITermination termination)
    {
        BestCandidate = adam;
        Evaluator = evaluator;
        Termination = termination;

        State = OptimizerState.NotStarted;
        m_lock = new object();
        stopRequested = pauseRequested = false;
        GenerationsNumber = 0; 
        TimeEvolving = TimeSpan.Zero;
    }
    /// <summary>
    /// Pauses the optimizer.
    /// </summary>
    public virtual void Pause()
    {
        lock (m_lock)
        {
            State = OptimizerState.Paused;
        }
        OnPaused?.Invoke();
    }

    /// <summary>
    /// Resumes the optimizer.
    /// </summary>
    public virtual void Resume()
    {
        OnResumed?.Invoke();
        lock (m_lock)
        {
            State = OptimizerState.Resumed;
            pauseRequested = false;
        }

        Run();
    }

    /// <summary>
    /// Stops the optimizer.
    /// </summary>
    public virtual void Stop()
    {
        lock (m_lock)
        {
            State = OptimizerState.Stopped;
        }
        OnStopped?.Invoke();
    }

    /// <summary>
    /// Starts the optimizer.
    /// </summary>
    public virtual void Start()
    {
        OnStarted?.Invoke();
        lock (m_lock)
        {
            stopRequested = false;
            pauseRequested = false;
            State = OptimizerState.Started;
            clock = Stopwatch.StartNew();
        }

        Run();
    }

    public abstract void RunOnce();

    /// <summary>
    /// Runs the optimization algorithm until a termination condition is reached or the optimizer is paused or stopped.
    /// </summary>
    /// <returns>The best candidate solution found by the optimizer.</returns>
    public virtual IEvaluable Run()
    {
        while (!TerminatioReached() && !(State == OptimizerState.Paused || State == OptimizerState.Stopped))
        {
            if (stopRequested)
            {
                Stop();
                break;
            }
            if(pauseRequested)
            {
                Pause();
                break;
            }

            clock.Restart();
            RunOnce();
            OnGenerationRan?.Invoke();
            clock.Stop();
            State = OptimizerState.Running;
        }

        return BestCandidate;
    }

    /// <summary>
    /// Determines if the optimizer has reached a termination condition.
    /// </summary>
    /// <returns>True if the termination condition has been reached, false otherwise.</returns>
    private bool TerminatioReached()
    {
      /*  if(Termination.HasReached(this))
        {
            OnTerminationReached?.Invoke();
            return true;
        }*/
        return false;
      
    }

    /// <summary>
    /// Requests that the optimization process be stopped.
    /// </summary>
    public void RequestStop()
    {
        lock(m_lock)
        {
            stopRequested = true;
        }
    }

    /// <summary>
    /// Requests that the optimization process be paused.
    /// </summary>
    public void RequestPause()
    {
        lock (m_lock)
        {
            pauseRequested = true;
        }
    }

    public abstract string GetName();

}
