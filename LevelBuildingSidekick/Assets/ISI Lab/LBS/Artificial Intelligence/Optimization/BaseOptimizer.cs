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

        #region Fields
        protected Op_State state;
        protected Stopwatch clock;
        protected readonly object m_lock;
        protected bool stopRequested;
        protected bool pauseRequested;
        protected int generationsNumber;

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

        #region Properties

        public int GenerationsNumber => generationsNumber;

        public IOptimizable BestCandidate 
        {
            get => bestCandidate;
            set => bestCandidate = value; 
        }

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


        #region Events
        public Action OnGenerationRan { get; set; }
        public Action OnTerminationReached { get; set; }
        public Action OnStopped { get; set; }
        public Action OnResumed { get; set; }
        public Action OnPaused { get; set; }
        public Action OnStarted { get; set; }
        #endregion

        public BaseOptimizer()
        {
            generationsNumber = 0;
            m_lock = new object();
            stopRequested = pauseRequested = false;
            State = Op_State.NotStarted;

        }

        public BaseOptimizer(IOptimizable adam, IPopulation population, IEvaluator evaluator, ISelection selection, ITermination termination) : this()
        {
            Adam = adam;
            BestCandidate = Adam;
            Population = population;
            Evaluator = evaluator;
            Selection = selection;
            Termination = termination;
        }

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
            while(!TerminatioReached())
            {
                RunOnce();
                generationsNumber++;
                OnGenerationRan?.Invoke();
            }
        }

        public abstract List<IOptimizable> GetNeighbors( IOptimizable Adam);

        /*public virtual IEvaluable Run()
        {
            while (!TerminatioReached() && !(State == Op_State.Paused || State == Op_State.Stopped))
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
                OnGenerationRan?.Invoke();
                clock.Stop();
                State = Op_State.Running;
            }

            return BestCandidate;
        }*/

        /// <summary>
        /// Determines if the optimizer has reached a termination condition.
        /// </summary>
        /// <returns>True if the termination condition has been reached, false otherwise.</returns>
        public bool TerminatioReached()
        {
            if (Termination.HasReached(this))
            {
                BestCandidate = Population.CurrentGeneration.BestCandidate;
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