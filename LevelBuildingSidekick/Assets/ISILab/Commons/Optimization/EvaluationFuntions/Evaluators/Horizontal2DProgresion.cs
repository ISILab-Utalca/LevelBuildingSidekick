using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class Horizontal2DProgresion : Progression2DEvaluator
{
    public override string GetName()
    {
        return "Horizontal 2D progresion fitness";
    }

    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }

    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        float p = 0;
        int prev = 0;
        int max = 0;

        for (int i = 0; i < stmc.MatrixWidth; i++)
        {
            int current = 0;
            for (int j = 0; j < height; j++)
            {
                if (stmc.GetGene<int>(stmc.ToIndex(new Vector2(i, j))) == id)
                {
                    current++;
                }
            }
            if (current >= max)
            {
                max = current;
                p++;
            }

            if (current > prev)
            {
                p++;
            }
            prev = current;
        }

        p /= 2 * stmc.MatrixWidth;

        return Mathf.Clamp(p, MinValue, MaxValue);
    }
}