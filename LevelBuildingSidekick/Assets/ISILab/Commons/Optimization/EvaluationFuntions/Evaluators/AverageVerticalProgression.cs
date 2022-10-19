using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class AverageVerticalProgression : Progression2DEvaluator
{
    int stride;

    public override float EvaluateProgression(StampTileMapChromosome stmc, int id, int height)
    {
        int p = 0;
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

        p /= 2 * height;

        return p;
    }

    public override VisualElement CIGUI()
    {
        var content = new VisualElement();

        var intField = new IntegerField("Stride: ");
        intField.value = stride;
        intField.RegisterCallback<ChangeEvent<int>>((e) => stride = e.newValue);

        content.Add(base.CIGUI());
        content.Add(intField);

        return content;
    }

    public override string GetName()
    {
        return "Average Vertical Progression";
    }
}
