using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class Vertical2DProgresionFitness : IRangedEvaluator
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

        for (int j = 0; j < height; j++)
        {
            int current = 0;
            for (int i = 0; i < stmc.MatrixWidth; i++) 
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

        p /= height;

        return Mathf.Clamp(p, MinValue, MaxValue);
    }

    public string GetName()
    {
        return "Vertical 2D progresion fitness";
    }

    public VisualElement CIGUI()
    {
        var ve = new VisualElement();

        return ve;
    }
}
