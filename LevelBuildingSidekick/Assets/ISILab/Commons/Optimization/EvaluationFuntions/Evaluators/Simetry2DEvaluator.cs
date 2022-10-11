using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

[System.Serializable]
public abstract class Simetry2DEvaluator : IRangedEvaluator
{
    public abstract float MaxValue { get; }
    public abstract float MinValue { get; }

    public int matrixWidth;

    public Simetry2DEvaluator()
    {
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if(!(evaluable is ITileMap))
        {
            throw new FitnessException("evaluable must be ITileMap");
        }

        matrixWidth = (evaluable as ITileMap).MatrixWidth;

        var data = evaluable.GetDataSquence<object>();
        int height = data.Length / matrixWidth;

        float simetry = CalculateSimetry(data, height)/(data.Length / 2);

        return Mathf.Clamp(simetry, MinValue, MaxValue);
    }

    public abstract float CalculateSimetry(object[] data, int height);

    public abstract string GetName();
}
