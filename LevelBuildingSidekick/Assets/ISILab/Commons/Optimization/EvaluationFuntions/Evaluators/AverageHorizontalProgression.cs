using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class AverageHorizontalProgression : Progression2DEvaluator
{
    int stride = 1;

    /// <summary>
    /// Calculates the progression of a given tile map chromosome.
    /// </summary>
    /// <param name="stmc">The tile map chromosome to evaluate.</param>
    /// <param name="id">The ID of the tile to evaluate.</param>
    /// <param name="height">The height of the tile map.</param>
    /// <returns>A float representing the progression of the tile map.</returns>
    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        int p = 0;
        int prev = 0;
        int max = 0;

        for (int i = 0; i < stmc.MatrixWidth; i += stride)
        {
            int current = 0;
            for (int k = 0; k < stride; k++)
            {
                if (i + k >= stmc.MatrixWidth)
                {
                    break;
                }
                for (int j = 0; j < height; j++)
                {
                    if (stmc.GetGene<int>(stmc.ToIndex(new Vector2(i + k, j))) == id)
                    {
                        current++;
                    }
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

        return p;
    }

    /// <summary>
    /// Creates a visual element for the current object.
    /// </summary>
    /// <returns>A visual element representing the current object.</returns>
    public override VisualElement CIGUI()
    {
        var content = new VisualElement();

        var intField = new IntegerField("Stride: ");
        intField.value = stride;
        intField.RegisterCallback<ChangeEvent<int>>((e) => {
            if (e.newValue >= 1)
            {
                stride = e.newValue;
            }
            else
            {
                intField.value = 1;
                stride = 1;
            }
        });

        content.Add(base.CIGUI());
        content.Add(intField);

        return content;
    }

    public override string GetName()
    {
        return "Average Horizontal Progression";
    }
}
