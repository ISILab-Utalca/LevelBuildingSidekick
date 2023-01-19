using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class AverageVerticalProgression : Progression2DEvaluator
{
    int stride = 1;

    /// <summary>
    /// Evaluates the progression of the specified tile in the given chromosome.
    /// </summary>
    /// <param name="stmc">The chromosome to evaluate.</param>
    /// <param name="id">The ID of the tile to evaluate.</param>
    /// <param name="height">The height of the chromosome.</param>
    /// <returns>A float value representing the progression of the specified tile in the given chromosome.</returns>
    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        float p = 0;
        int prev = 0;
        int max = 0;

        for (int j = 0; j < height; j += stride)
        {
            int current = 0;
            for (int k = 0; k < stride; k++)
            {
                if (j + k >= height)
                {
                    break;
                }
                for (int i = 0; i < stmc.MatrixWidth; i++)
                {
                    if (stmc.GetGene<int>(stmc.ToIndex(new Vector2(i, j + k))) == id)
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

        p /=  (2 * height);

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

    /// <summary>
    /// Gets the name of the evaluator.
    /// </summary>
    /// <returns> The name of the evaluator. </returns>
    public override string GetName()
    {
        return "Average Vertical Progression";
    }
}
