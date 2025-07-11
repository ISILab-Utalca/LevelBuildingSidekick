using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.AI.Categorization
{

    [System.Serializable]
    public class MapElites : ICloneable
    {
        #region FIELDS

        [SerializeField]
        private int xSampleCount = 4;

        [SerializeField]
        private int ySampleCount = 4;

        [SerializeField, Range(0, 0.5f), HideInInspector]
        public double devest = 0.5;

        [SerializeField, SerializeReference, HideInInspector]
        IRangedEvaluator xEvaluator;
        Vector2 xThreshold = new Vector2(0.2f, 0.8f);

        [SerializeReference, HideInInspector]
        IRangedEvaluator yEvaluator;
        Vector2 yThreshold = new Vector2(0.2f, 0.8f);

        [SerializeReference, HideInInspector]
        BaseOptimizer optimizer = new GeneticAlgorithm();

        [HideInInspector]
        public List<int> changedSample;

        private Thread thread;

        #endregion

        #region FIELDS

        /// <summary>
        /// Gets or sets the number of samples in the X dimension.
        /// </summary>
        /// <value>The number of samples in the X dimension.</value>
        public int XSampleCount
        {
            get
            {
                return xSampleCount;
            }
            set
            {
                if (xSampleCount != value && value > 0)
                {
                    xSampleCount = value;
                    OnSampleSizeChange();
                }
            }
        }


        /// <summary>
        /// Gets or sets the number of samples in the Y dimension.
        /// </summary>
        /// <value>The number of samples in the Y dimension.</value>
        public int YSampleCount
        {
            get
            {
                return ySampleCount;
            }
            set
            {
                if (ySampleCount != value && value > 0)
                {
                    ySampleCount = value;
                    OnSampleSizeChange();
                }
            }
        }

        public IOptimizable Adam
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the evaluator for the X dimension.
        /// </summary>
        /// <value>The evaluator for the X dimension.</value>
        public IRangedEvaluator XEvaluator
        {
            get
            {
                return xEvaluator;
            }
            set
            {
                if (xEvaluator == null || !xEvaluator.Equals(value))
                {
                    xEvaluator = value;
                    OnEvaluatorChange();
                }
            }
        }

        public Vector2 XThreshold
        {
            get => xThreshold;
            set
            {
                var v = value;
                v.x = v.x < 0 ? 0 : v.x;
                v.y = v.y < 0 ? 0 : v.y;
                v.x = v.x > 1 ? 1 : v.x;
                v.y = v.y > 1 ? 1 : v.y;
                v.x = v.x > v.y ? v.y : v.x;

                xThreshold = v;
            }
        }

        /// <summary>
        /// Gets or sets the evaluator for the Y dimension.
        /// </summary>
        /// <value>The evaluator for the Y dimension.</value>
        public IRangedEvaluator YEvaluator
        {
            get
            {
                return yEvaluator;
            }
            set
            {
                if (yEvaluator == null || !yEvaluator.Equals(value))
                {
                    yEvaluator = value;
                    OnEvaluatorChange();
                }
            }
        }

        public Vector2 YThreshold
        {
            get => yThreshold;
            set
            {
                var v = value;
                v.x = v.x < 0 ? 0 : v.x;
                v.y = v.y < 0 ? 0 : v.y;
                v.x = v.x > 1 ? 1 : v.x;
                v.y = v.y > 1 ? 1 : v.y;
                v.x = v.x > v.y ? v.y : v.x;

                yThreshold = v;
            }
        }

        public IOptimizable[,] BestSamples { get; private set; }

        /// <summary>
        /// Gets or sets the optimizer to use.
        /// </summary>
        /// <value>The optimizer to use.</value>
        public BaseOptimizer Optimizer
        {
            get
            {
                return optimizer;
            }
            set
            {
                optimizer = value;
                OnOptimizerChange();
            }
        }

        public bool Running => Optimizer.State != Op_State.TerminationReached && Optimizer.State != Op_State.Stopped && Optimizer.State != Op_State.Paused && Optimizer.State != Op_State.NotStarted;

        public bool Finished => Optimizer.State == Op_State.TerminationReached;

        #endregion

        #region EVENTS

        public Action OnEnd;

        Action OnSampleSizeChanged;

        Action OnEvaluatorChanged;

        public Action OnOptimizerChanged;

        public Action<Vector2Int> OnSampleUpdated;

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="MapElites"/> class.
        /// </summary>
        public MapElites()
        {
            xSampleCount = 5;
            ySampleCount = 5;
            devest = 0.5;
            BestSamples = new IOptimizable[xSampleCount, ySampleCount];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapElites"/> class with the specified evaluators.
        /// </summary>
        /// <param name="xEvaluator">The evaluator for the X dimension.</param>
        /// <param name="yEvaluator">The evaluator for the Y dimension.</param>
        public MapElites(IRangedEvaluator xEvaluator, IRangedEvaluator yEvaluator, BaseOptimizer optimizer)
        {
            this.xEvaluator = xEvaluator;
            this.yEvaluator = yEvaluator;
            this.optimizer = optimizer;
            xSampleCount = 5;
            ySampleCount = 5;
            devest = 0.5;
            BestSamples = new IOptimizable[xSampleCount, ySampleCount];
        }

        /// <summary>
        /// Sets the "Adam" property of the "Optimizer" ,
        /// Evaluates the fitness function of "Adam", invokes the "Clear" method, and subscribes
        /// "OnGenerationRan" and "OnTerminationReached" events of the "Optimizer" property. 
        /// Also creates a new thread with the "Start" method of the "Optimizer" property as its argument and starts the thread.
        /// </summary>
        public void Run()
        {
            if (Optimizer.State != Op_State.NotStarted)
            {
                Debug.LogWarning("[ISI Lab] Process Already Running. State: " + Optimizer.State);
                return;
            }

            Optimizer.Adam = Adam;
            Clear();
            SetupCallbacks();

            thread = new Thread(Optimizer.Start);
            TryStartThread();
        }

        public void Restart()
        {
            SetupCallbacks();

            thread = new Thread(Optimizer.Restart);
            TryStartThread();
        }

        private void SetupCallbacks()
        {
            Optimizer.OnGenerationRan = () =>
            {
                OLDUpdateSamples(Optimizer.LastGeneration);
            };

            Optimizer.OnTerminationReached = () =>
            {
                int count = UpdateBestSamples();
                if (Running)
                {
                    thread.Join(); // or thread.Abort() depending on context
                }

                //Debug.Log("Finished: " + count);
                Optimizer.State = Op_State.TerminationReached;
                OnEnd?.Invoke();
            };
        }

        private int UpdateBestSamples()
        {
            int c = 0;
            for (int j = 0; j < BestSamples.GetLength(1); j++)
            {
                for (int i = 0; i < BestSamples.GetLength(0); i++)
                {
                    if (BestSamples[j, i] != null)
                    {
                        c++;
                        UpdateSample(i, j, BestSamples[j, i]);
                    }
                }
            }
            return c;
        }

        private void TryStartThread()
        {
            try
            {
                thread.Start();
            }
            catch
            {
                thread.Abort();
            }
        }


        public void Stop()
        {
            //TODO: Investigate what parts break if this gets to be used
            thread = new Thread(Optimizer.Stop);
            thread.Start();
        }

        public void UpdateSamples(IOptimizable[] samples)
        {
            var evaluables = MapSamples(samples);

            int totalCells = xSampleCount * ySampleCount; 
            if (evaluables.Count > totalCells)
            {
                //Debug.LogWarning("More evaluables than grid cells. Some evaluables will be ignored.");
                evaluables = evaluables.Take(totalCells).ToList(); // Prevent overflow
            }

            // Sort evaluables by xFitness, then yFitness
            evaluables = evaluables
                .OrderBy(e => e.xFitness)
                .ThenBy(e => e.yFitness)
                .ToList();

            for (int i = 0; i < evaluables.Count; i++)
            {
                int x = i % xSampleCount;
                int y = i / xSampleCount;

                var me = evaluables[i];

                //Debug.Log("Sorted assignment -> pos: " + x + "," + y);
                me.evaluable.xFitness = evaluables[i].xFitness;
                me.evaluable.yFitness = evaluables[i].yFitness;
                UpdateSample(x, y, me.evaluable);
            }
        }

        /// <summary>
        /// Updates the best samples in the map with the provided array of evaluables.
        /// </summary>
        /// <param name="samples">The array of evaluables to update the map with.</param>
        public void OLDUpdateSamples(IOptimizable[] samples)
        {
            var max = samples.Select(o => o.Fitness).OrderBy(n => n).ToArray();
            var evaluables = MapSamples(samples);
   
            float xT = Mathf.Abs(XEvaluator.MaxValue - XEvaluator.MinValue);
            var xLowest = XEvaluator.MinValue + xT * xThreshold.x;
            var xHighest = XEvaluator.MinValue + xT * xThreshold.y;

            var xStep = (xHighest - xLowest) / XSampleCount;

            float yT = Mathf.Abs(YEvaluator.MaxValue - YEvaluator.MinValue);
            var yLowest = YEvaluator.MinValue + yT * yThreshold.x;
            var yHighest = YEvaluator.MinValue + yT * yThreshold.y;

            float yStep = (yHighest - yLowest) / YSampleCount;
            
            foreach (var me in evaluables)
            {
                var xPos = (me.xFitness - xLowest) / xStep;
                if (xPos < 0)
                    xPos = 0;
                if (xPos >= xSampleCount)
                    xPos = xSampleCount - 1;

                var yPos = (me.yFitness - yLowest) / yStep;
                
                if (yPos < 0)
                    yPos = 0;
                if (yPos >= ySampleCount)
                    yPos = ySampleCount - 1;

                me.evaluable.xFitness = me.xFitness;
                me.evaluable.yFitness = me.yFitness;
                                
                 if (me.evaluable != optimizer.Adam) {
                    UpdateSample((int)xPos, (int)yPos, me.evaluable);
                }
                
            }
        }

        /// <summary>
        /// Updates the sample at the specified coordinates with the provided evaluable.
        /// </summary>
        /// <param name="x">X index of element to update in the "BestSamples" array.</param>
        /// <param name="y">Y index of element to update in the "BestSamples" array.</param>
        /// <param name="evaluable">Evaluable object to update in the "BestSamples" array.</param>
        /// <returns>Boolean value indicating if the update was successful or not.
        public bool UpdateSample(int x, int y, IOptimizable evaluable)
        {
            var current = BestSamples[y, x];
            if (current == null || evaluable.Fitness > current.Fitness)
            {
                BestSamples[y, x] = evaluable.Clone() as IOptimizable;
                BestSamples[y, x].Fitness = evaluable.Fitness;
                //Debug.Log($"EVALUABLE ({y}, {x})");
                //Debug.Log("Fitness: " + evaluable.Fitness);
                BestSamples[y, x].yFitness = evaluable.yFitness;
                //Debug.Log("Fitness Y: " + evaluable.yFitness);
                BestSamples[y, x].xFitness = evaluable.xFitness;
                //Debug.Log("Fitness X: " + evaluable.xFitness);

                OnSampleUpdated?.Invoke(new Vector2Int(x, y));
                return true;
            }

            return false;
        }


        /// <summary>
        /// Maps the provided array of evaluables to a list of evaluables with x and y fitness values.
        /// </summary>
        /// <param name="samples">The array of evaluables to map.</param>
        /// <returns>A list of evaluables with x and y fitness values.</returns>
        public List<MappedIEvaluable> MapSamples(IOptimizable[] samples)
        {
            List<MappedIEvaluable> evaluables = new List<MappedIEvaluable>();

            foreach (var s in samples)
            {
                evaluables.Add(new MappedIEvaluable(s, XEvaluator.Evaluate(s), YEvaluator.Evaluate(s)));
            }

            return evaluables;
        }

        /// <summary>
        /// Calls the "Clear" method and raises the "OnSampleSizeChanged" event if it is not null.
        /// </summary>
        private void OnSampleSizeChange()
        {
            Clear();
            OnSampleSizeChanged?.Invoke();
        }

        /// <summary>
        /// Calls the "Clear" method and raises the "OnEvaluatorChanged" event if it is not null.
        /// </summary>
        private void OnEvaluatorChange()
        {
            Clear();
            OnEvaluatorChanged?.Invoke();
        }

        /// <summary>
        /// Calls the "Clear" method and raises the "OnOptimizerChanged" event if it is not null.
        /// </summary>
        private void OnOptimizerChange()
        {
            Clear();
            OnOptimizerChanged?.Invoke();
        }

        /// <summary>
        /// Sets "BestSamples" to a new two-dimensional array of "IEvaluable" objects with dimensions specified
        /// by the "xSampleCount" and "ySampleCount" properties of the current class.
        /// </summary>
        private void Clear()
        {
            BestSamples = new IOptimizable[xSampleCount, ySampleCount];
        }

        public object Clone()
        {
            var me = new MapElites();
            me.optimizer = optimizer.Clone() as BaseOptimizer;
            me.xSampleCount = xSampleCount;
            me.ySampleCount = ySampleCount;
            me.XThreshold = XThreshold;
            me.YThreshold = YThreshold;
            me.devest = devest;
            me.XEvaluator = xEvaluator.Clone() as IRangedEvaluator;
            me.YEvaluator = yEvaluator.Clone() as IRangedEvaluator;
            return me;
        }
    }

    /// <summary>
    /// Struct to store and represent an "IEvaluable" object along with its "xFitness" and "yFitness" values.
    /// </summary>
    public struct MappedIEvaluable
    {
        public float xFitness;
        public float yFitness;
        public IOptimizable evaluable;

        public MappedIEvaluable(IOptimizable evaluable, float xFitness, float yFitness)
        {
            this.evaluable = evaluable;
            this.xFitness = xFitness;
            this.yFitness = yFitness;
        }
    }
}
