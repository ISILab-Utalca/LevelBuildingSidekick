using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using System.Linq;
using System;
using UnityEngine.UIElements;
using Commons.Optimization;
using System.Threading;

[System.Serializable]
public class MapElites
{
    #region FIELDS

    private int xSampleCount = 4;

    private int ySampleCount = 4;

    [Range(0, 0.5f)]
    public double devest = 0.5;

    [SerializeField, SerializeReference]
    IRangedEvaluator xEvaluator;

    [SerializeReference]
    IRangedEvaluator yEvaluator;

    [SerializeReference]
    BaseOptimizer optimizer = new GeneticAlgorithm();

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
            if(xSampleCount != value && value > 0)
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
            if(ySampleCount != value && value > 0)
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

    public bool Running => thread != null && thread.IsAlive && thread.ThreadState == ThreadState.Running;

    #endregion

    #region EVENTS

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
        //xEvaluator = new Vertical2DSimetry();
        //yEvaluator = new Horizontal2DSimetry();
        optimizer = new GeneticAlgorithm();
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
    public MapElites(IRangedEvaluator xEvaluator, IRangedEvaluator yEvaluator)
    {
        this.xEvaluator = xEvaluator;
        this.yEvaluator = yEvaluator;
        optimizer = new GeneticAlgorithm();
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
        Optimizer.Adam = Adam;
        var fitness = Optimizer.Evaluator.Evaluate(Adam);
        Clear();
        Optimizer.OnGenerationRan += () =>
        { 
            UpdateSamples(Optimizer.LastGeneration); 
        };
        Optimizer.OnTerminationReached += () =>
        {
            int c = 0;
            for (int j = 0; j < BestSamples.GetLength(1); j++)
            {
                for (int i = 0; i < BestSamples.GetLength(0); i++)
                {
                    if(BestSamples[i,j] != null)
                    {
                        c++;
                        UpdateSample(i,j,BestSamples[i,j]);
                    }
                }
            }
            if(Running)
            {
                thread.Join();
            }
            Debug.Log("Finished: " + c);

        };
        thread = new Thread(Optimizer.Start);
        thread.Start(); 
    }

    /// <summary>
    /// Updates the best samples in the map with the provided array of evaluables.
    /// </summary>
    /// <param name="samples">The array of evaluables to update the map with.</param>
    public void UpdateSamples(IOptimizable[] samples)
    {
        
        var evaluables = MapSamples(samples);

        float xStep = Mathf.Abs(XEvaluator.MaxValue - XEvaluator.MinValue) / XSampleCount;
        float yStep = Mathf.Abs(YEvaluator.MaxValue - YEvaluator.MinValue) / YSampleCount;

        foreach (var me in evaluables)
        {
            var xPos = (me.xFitness - XEvaluator.MinValue) / xStep;
            var yPos = (me.yFitness - YEvaluator.MinValue) / yStep;


            var tileXPos = (int)xPos;
            var tileYPos = (int)yPos;

            var dx = Mathf.Abs(0.5f - (xPos - tileXPos));
            var dy = Mathf.Abs(0.5f - (yPos - tileYPos));


            if (dx <= devest && dy <= devest)
            {
                tileXPos = tileXPos >= XSampleCount ? tileXPos - 1 : tileXPos;
                tileYPos = tileYPos >= YSampleCount ? tileYPos - 1 : tileYPos;
                UpdateSample(tileXPos, tileYPos, me.evaluable);
            }

            //Debug.Log(xPos + " - " + yPos);
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
        if (BestSamples[x,y] == null)
        {
            BestSamples[x, y] = evaluable;
            OnSampleUpdated?.Invoke(new Vector2Int(x,y));
            return true;
        }

        if(BestSamples[x,y].Fitness <= evaluable.Fitness)
        {
            BestSamples[x, y] = evaluable;
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
        //Optimizer.OnGenerationRan += () => UpdateSamples(Optimizer.LastGeneration);
    }

    /// <summary>
    /// Sets "BestSamples" to a new two-dimensional array of "IEvaluable" objects with dimensions specified
    /// by the "xSampleCount" and "ySampleCount" properties of the current class.
    /// </summary>
    private void Clear()
    {
        BestSamples = new IOptimizable[xSampleCount, ySampleCount];
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
