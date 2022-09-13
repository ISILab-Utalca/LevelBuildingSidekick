using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BaseOptimizer : IOptimizer
{

    #region Properties
    public int GenerationsNumber { get; private set; }
    public bool IsRunning { get { return State == OptimizerState.Running || State == OptimizerState.Started || State == OptimizerState.Resumed; } }
    public TimeSpan TimeEvolving { get; private set; }

    public IEvaluable BestCandidate { get; private set; }


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

    public virtual void Pause()
    {
        lock (m_lock)
        {
            State = OptimizerState.Paused;
        }
        OnPaused?.Invoke();
    }

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

    public virtual void Stop()
    {
        lock (m_lock)
        {
            State = OptimizerState.Stopped;
        }
        OnStopped?.Invoke();
    }

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

    private bool TerminatioReached()
    {
        if(Termination.HasReached(this))
        {
            OnTerminationReached?.Invoke();
            return true;
        }
        return false;
    }

    public void RequestStop()
    {
        lock(m_lock)
        {
            stopRequested = true;
        }
    }

    public void RequestPause()
    {
        lock (m_lock)
        {
            pauseRequested = true;
        }
    }


}
