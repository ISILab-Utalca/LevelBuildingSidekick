using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

[System.Serializable]
public class Horizontal2DSimetryEvaluator : Simetry2DEvaluator
{
    public override float MaxValue => 1;

    public override float MinValue => 0;

    public Horizontal2DSimetryEvaluator() : base() { }

    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        for (int j = 0; j < height / 2; j++)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                if (data[(matrixWidth * j) + i].Equals(data[(height - matrixWidth * j) + i]))
                {
                    simetry++;
                }
            }
        }
        return simetry;
    }

    public override string GetName()
    {
        return "Horizontal 2D simetry";
    }
}
