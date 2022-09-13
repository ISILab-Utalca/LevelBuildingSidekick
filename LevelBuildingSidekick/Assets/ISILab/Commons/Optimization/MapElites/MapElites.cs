using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using System.Linq;
using System;

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
                OnSampleSizeChanged?.Invoke();
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
                OnSampleSizeChanged?.Invoke();
            }
        }
    }

    System.Action OnSampleSizeChanged;

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
            if(!xEvaluator.Equals(value))
            {
                xEvaluator = value;
                OnEvaluatorChanged?.Invoke();
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
            if(!yEvaluator.Equals(value))
            {
                yEvaluator = value;
                OnEvaluatorChanged?.Invoke();
            }
        }
    }

    System.Action OnEvaluatorChanged;

    public MapElites()
    {
        xEvaluator = null;
        yEvaluator = null;
        xSampleCount = 5;
        ySampleCount = 5;
        threshold = 0.25;
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
        OnSampleSizeChanged += OnSampleSizeChange;
        OnEvaluatorChanged += OnEvaluatorChange;
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
            return true;
        }

        if(BestSamples[x,y].Fitness < evaluable.Fitness)
        {
            BestSamples[x, y] = evaluable;
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

    public void OnSampleSizeChange()
    {
        Clear();
    }

    public void OnEvaluatorChange()
    {
        Clear();
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
