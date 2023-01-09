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
    /// Calculates the symmetry of an object represented in a matrix.
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

    /// <summary>
    /// Gets the name of the evaluator.
    /// </summary>
    /// <returns> The name of the evaluator. </returns>
    public override string GetName()
    {
        return "Vertical 2D Simetry";
    }

    /// <summary>
    /// Generates and returns a visual element for use in the Unity editor.
    /// </summary>
    /// <returns> A <see cref="VisualElement"/>. </returns>
    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }
}
