using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class Vertical2DProgresion : Progression2DEvaluator
{
    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }

    /// <summary>
    /// Calculates the progression of a certain id in a <see cref="StampTileMapChromosome"/>.
    /// </summary>
    /// <param name="stmc">The <see cref="StampTileMapChromosome"/> to evaluate.</param>
    /// <param name="id">The id to evaluate the progression of.</param>
    /// <param name="height">The height of the <see cref="StampTileMapChromosome"/>.</param>
    /// <returns>A float value representing the progression of the id in the <see cref="StampTileMapChromosome"/>.</returns>
    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        float p = 0;
        int prev = 0;
        int max = 0;

        for (int j = 0; j < height; j++)
        {
            int current = 0;
            for (int i = 0; i < stmc.MatrixWidth; i++)
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

        p /= (2 * height);

        return Mathf.Clamp(p, MinValue, MaxValue);
    }

    public override string GetName()
    {
        return "Vertical 2D progresion fitness";
    }
}
