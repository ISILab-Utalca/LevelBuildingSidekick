using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using System.Linq;
using System;

[System.Serializable]
public class MapElites
{
    private int xSampleCount;
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

    private int ySampleCount;
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

    Action OnSampleSizeChanged;

    [Range(0,0.5f)]
    public double threshold;

    public IEvaluable[,] BestSamples { get; private set; }

    IRangedEvaluator xEvaluator;
    public IRangedEvaluator XEvaluator
    {
        get
        {
            return xEvaluator;
        }
        set
        {
            if(xEvaluator == null || !xEvaluator.Equals(value))
            {
                xEvaluator = value;
                OnEvaluatorChange();
            }
        }
    }

    IRangedEvaluator yEvaluator;
    public IRangedEvaluator YEvaluator
    {
        get
        {
            return yEvaluator;
        }
        set
        {
            if(yEvaluator == null || !yEvaluator.Equals(value))
            {
                yEvaluator = value;
                OnEvaluatorChange();
            }
        }
    }

    Action OnEvaluatorChanged;

    IOptimizer optimizer;
    public IOptimizer Optimizer
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

    public System.Action OnOptimizerChanged;

    public Action<Vector2Int> OnSampleUpdated;

    public MapElites()
    {
        xEvaluator = null;
        yEvaluator = null;
        xSampleCount = 5;
        ySampleCount = 5;
        threshold = 0.25;
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
    }

    public MapElites(IRangedEvaluator xEvaluator, IRangedEvaluator yEvaluator)
    {
        xSampleCount = 5;
        ySampleCount = 5;
        threshold = 0.25;
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
        OnSampleSizeChanged += OnSampleSizeChange;
        OnEvaluatorChanged += OnEvaluatorChange;
    }

    public void Run()
    {
        optimizer.Start();
    }

    public void UpdateSamples(IEvaluable[] samples)
    {
        var evaluables = MapSamples(samples);

        float xStep = Mathf.Abs(XEvaluator.MaxValue - XEvaluator.MinValue) / XSampleCount;
        float yStep = Mathf.Abs(YEvaluator.MaxValue - YEvaluator.MinValue) / YSampleCount;

        for (int j = 0; j < YSampleCount; j++)
        {
            for(int i = 0; i < XSampleCount; i++)
            {
                var dist = (xStep + yStep)*threshold;
                foreach(var me in evaluables)
                {
                    var d = Mathf.Abs(xStep * i - me.xFitness) + Mathf.Abs(yStep * j - me.yFitness);
                    if (d < dist)
                    {
                        dist = d;
                        UpdateSample(i, j, me.evaluable);
                    }
                }
            }
        }
    }

    public bool UpdateSample(int x, int y, IEvaluable evaluable)
    {
        if(BestSamples[x,y] == null)
        {
            BestSamples[x, y] = evaluable;
            OnSampleUpdated?.Invoke(new Vector2Int(x,y));
            return true;
        }

        if(BestSamples[x,y].Fitness < evaluable.Fitness)
        {
            BestSamples[x, y] = evaluable;
            OnSampleUpdated?.Invoke(new Vector2Int(x, y));
            return true;
        }
        return false;
    }

    public List<MappedIEvaluable> MapSamples(IEvaluable[] samples)
    {
        List<MappedIEvaluable> evaluables = new List<MappedIEvaluable>();

        foreach (var s in samples)
        {
            evaluables.Add(new MappedIEvaluable(s, XEvaluator.Evaluate(s), YEvaluator.Evaluate(s)));
        }

        return evaluables;
    }

    private void OnSampleSizeChange()
    {
        Clear();
        OnSampleSizeChanged?.Invoke();
    }

    private void OnEvaluatorChange()
    {
        Clear();
        OnEvaluatorChanged?.Invoke();
    }

    private void OnOptimizerChange()
    {
        Clear();
        Optimizer.OnGenerationRan += () => UpdateSamples(Optimizer.LastGeneration);
    }

    private void Clear()
    {
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
    }
}

public struct MappedIEvaluable
{
    public float xFitness;
    public float yFitness;
    public IEvaluable evaluable;

    public MappedIEvaluable(IEvaluable evaluable, float xFitness, float yFitness)
    {
        this.evaluable = evaluable;
        this.xFitness = xFitness;
        this.yFitness = yFitness;
    }
}
