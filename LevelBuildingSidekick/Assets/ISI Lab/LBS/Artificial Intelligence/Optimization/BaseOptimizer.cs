using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using System;
using GeneticSharp.Domain.Populations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using Commons.Optimization;
using GeneticSharp.Domain.Selections;

namespace Commons.Optimization
{
    public abstract class BaseOptimizer
    {

        #region FIELDS

        protected Op_State state;
        protected Stopwatch clock;
        protected readonly object m_lock;
        protected bool stopRequested;
        protected bool pauseRequested;

        [SerializeField, SerializeReference]
        IPopulation population;
        [SerializeField, SerializeReference]
        IEvaluator evaluator;
        [SerializeField, SerializeReference]
        ISelection selection;
        [SerializeField, SerializeReference]
        ITermination termination;

        protected IOptimizable adam;
        protected IOptimizable bestCandidate;
        
        #endregion

        #region PROPERTIES

        public int GenerationsNumber => Population.GenerationsNumber;

        public IOptimizable BestCandidate => Population.BestCandidate;

        public IOptimizable Adam 
        {
            get => adam;
            set => adam = value; 
        }

        public TimeSpan TimeEvolving => clock.Elapsed;

        public bool IsRunning => State == Op_State.Running || State == Op_State.Started || State == Op_State.Resumed; 

        public IPopulation Population
        {
            get => population;
            set => population = value;
        }

        public ITermination Termination
        {
            get => termination;
            set => termination = value;
        }

        public IEvaluator Evaluator
        {
            get => evaluator;
            set => evaluator = value;
        }

        public ISelection Selection
        {
            get => selection;
            set => selection = value;
        }

        public Op_State State
        {
            get
            {
                return state;
            }
            set
            {
                var shouldStop = OnStopped != null && state != value && value == Op_State.Stopped;

                state = value;

                if (shouldStop)
                {
                    Stop();
                }
            }
        }

        public IOptimizable[] LastGeneration
        {
            get
            {
                return Population.CurrentGeneration.Evaluables.ToArray();
            }
        }



        #endregion

        #region EVENTS
        public Action OnGenerationRan { get; set; }
        public Action OnTerminationReached { get; set; }
        public Action OnStopped { get; set; }
        public Action OnResumed { get; set; }
        public Action OnPaused { get; set; }
        public Action OnStarted { get; set; }
        #endregion

        #region CONSTRUCTOR

        public BaseOptimizer()
        {
            m_lock = new object();
            stopRequested = pauseRequested = false;
            State = Op_State.NotStarted;
        }

        public BaseOptimizer(IPopulation population, IEvaluator evaluator, ISelection selection, ITermination termination) : this()
        {
            Adam = population.Adam;
            Evaluator = evaluator;
            Selection = selection;
            Termination = termination;
            Population = population;
        }

        #endregion

        #region METHODS

        public virtual void Pause()
        {
            lock (m_lock)
            {
                State = Op_State.Paused;
                pauseRequested = true;
            }
            OnPaused?.Invoke();
        }

        public virtual void Resume()
        {
            OnResumed?.Invoke();
            lock (m_lock)
            {
                State = Op_State.Resumed;
                pauseRequested = false;
            }

            Run();
        }

        public virtual void Stop()
        {
            lock (m_lock)
            {
                State = Op_State.Stopped;
                stopRequested = true;
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
                State = Op_State.Started;
                clock = Stopwatch.StartNew();
            }

            Run();
        }

        public abstract void RunOnce ();

        public void Run()
        {
            int iterations = 0;
            Init();
            while(!TerminationReached() && !(State == Op_State.Paused || State == Op_State.Stopped) && iterations < 1000)
            {
                if (stopRequested)
                {
                    Stop();
                    break;
                }
                if (pauseRequested)
                {
                    Pause();
                    break;
                }

                clock.Restart();
                RunOnce();
                clock.Stop();
                OnGenerationRan?.Invoke();
                State = Op_State.Running;

                iterations++;
            }
        }

        void Init()
        {
            State = Op_State.Started;
            clock = new Stopwatch();
            clock.Start();
            Adam.Fitness = Evaluator.Evaluate(Adam);
            Population.Adam = Adam;
            Population.CreateInitialGeneration();
            clock.Stop();
        }

        /// <summary>
        /// Determines if the optimizer has reached a termination condition.
        /// </summary>
        /// <returns>True if the termination condition has been reached, false otherwise.</returns>
        public bool TerminationReached()
        {
            if (Termination.HasReached(this))
            {
                OnTerminationReached?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Requests that the optimization process be stopped.
        /// </summary>
        public void RequestStop()
        {
            lock (m_lock)
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

        #endregion
    }

    public enum Op_State
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