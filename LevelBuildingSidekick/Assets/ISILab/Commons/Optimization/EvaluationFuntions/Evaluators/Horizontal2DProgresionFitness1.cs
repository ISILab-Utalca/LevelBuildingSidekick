using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

public class Horizontal2DProgresionFitness : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public StampData stamp;

    public float Evaluate(IEvaluable evaluable)
    {
        if(!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;

        float p = 0;
        var id = stmc.stamps.FindIndex(s => s == stamp);
        var height = stmc.Length / stmc.MatrixWidth;

        int prev = 0;

        for (int i = 0; i < stmc.MatrixWidth; i++)
        {
            int current = 0;
            for (int j = 0; j < height; j++)
            {
                if(stmc.GetGene<int>(stmc.ToIndex(new Vector2(i,j))) == id)
                {
                    current++;
                }
            }
            if(current > prev)
            {
                p++;
            }
        }

        p /= stmc.MatrixWidth;

        return Mathf.Clamp(p, MinValue, MaxValue);
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
