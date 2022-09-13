using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

public abstract class Simetry2DEvaluator : IRangedEvaluator
{
    public abstract float MaxValue { get; }
    public abstract float MinValue { get; }

    public int matrixWidth;

    public Simetry2DEvaluator(int matrixWidth)
    {
        this.matrixWidth = matrixWidth;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        var data = evaluable.GetDataSquence<object>();
        int height = data.Length / matrixWidth;

        float simetry = CalculateSimetry(data, height)/(data.Length / 2);

        return Mathf.Clamp(simetry, MinValue, MaxValue);
    }

    public abstract float CalculateSimetry(object[] data, int height);
}
