using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public class Vertical2DSimetry : Simetry2DEvaluator
{
    public Vertical2DSimetry() : base() { }

    /// <summary>
    /// Calculates the simetry of the given data.
    /// </summary>
    /// <param name="data">The data to calculate simetry for.</param>
    /// <param name="height">The height of the data.</param>
    /// <returns>The simetry of the data.</returns>
    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        float elements = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                if ((int)(object)data[matrixWidth * j + i] == -1)
                    continue;
                elements++;
                if (data[(matrixWidth * j) + i].Equals(data[(height - 1 - j)*matrixWidth + i]))
                {
                    simetry++;
                }
            }
        }
        return simetry/elements;
    }

    public override string GetName()
    {
        return "Vertical 2D Simetry";
    }

    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }
}
