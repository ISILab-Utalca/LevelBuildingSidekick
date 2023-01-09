using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class Horizontal2DProgresion : Progression2DEvaluator
{
    public override string GetName()
    {
        return "Progression: Horizontal 2D";
    }

    /// <summary>
    /// Returns the custom inspector GUI for this object.
    /// </summary>
    /// <returns>A <see cref="VisualElement"/> representing the custom inspector GUI.</returns>
    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }


    /// <summary>
    /// Evaluates the progression of a given object in a matrix represented by a <see cref="StampTileMapChromosome"/> object.
    /// </summary>
    /// <param name="stmc">The <see cref="StampTileMapChromosome"/> object representing the matrix.</param>
    /// <param name="id">The identifier of the object being evaluated.</param>
    /// <param name="height">The number of rows in the matrix.</param>
    /// <returns>A value between <see cref="MinValue"/> and <see cref="MaxValue"/> </returns>
    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        int[] ocurrences = new int[stmc.MatrixWidth];
        float p = 0;

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
            ocurrences[i] = current;
        }

        for (int i = 1; i < ocurrences.Length; i++)
        {
            if(ocurrences[i] > ocurrences[i-1])
            {
                p++;
            }
        }


        p /= stmc.MatrixWidth;


        return Mathf.Clamp(p, MinValue, MaxValue);
    }
}