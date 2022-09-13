using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

public class Vertical2DSimetryEvaluator : Simetry2DEvaluator
{
    public override float MaxValue => 1;
    
    public override float MinValue => 0;

    public Vertical2DSimetryEvaluator(int matrixWidth) : base(matrixWidth) { }

    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth / 2; i++)
            {
                if (data[matrixWidth * j + i].Equals(data[matrixWidth * j + (matrixWidth - i)]))
                {
                    simetry++;
                }
            }
        }
        return simetry;
    }
}
