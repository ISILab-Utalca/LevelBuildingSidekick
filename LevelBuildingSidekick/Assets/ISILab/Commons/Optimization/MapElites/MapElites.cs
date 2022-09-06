using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Fitness;
using System.Linq;

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
            xSampleCount = value;
        }
    }

    private int ySampleCount;
    public int YSampleCount
    {
        get
        {
            return YSampleCount;
        }
        set
        {
            YSampleCount = value;
        }
    }

    public IEvaluable[,] BestSamples { get; private set; }

    public IEvaluator xEvaluator;
    public IEvaluator yEvaluator;

    public void UpdateSamples(IEvaluable[] samples)
    {
        var xSamples = samples.OrderBy(s => xEvaluator.Evaluate(s));
        var ySamples = samples.OrderBy(s => xEvaluator.Evaluate(s));
    }


}
