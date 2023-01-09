using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public class Horizontal2DSimetry : Simetry2DEvaluator
{
    public Horizontal2DSimetry() : base() { }

    /// <summary>
    /// Calculates the symmetry of an object represented in a matrix.
    /// </summary>
    /// <param name="data">A matrix representing the object, where each element is of type <see cref="object"/>.</param>
    /// <param name="height">The number of rows in the matrix.</param>
    /// <returns>A value between 0 and 1, representing the symmetry of the object. 0 indicates no symmetry and 1 indicates perfect symmetry.</returns>
    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        float elements = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth / 2; i++)
            {
                if ((int)(object)data[matrixWidth * j + i] == -1)
                    continue;
                elements++;
                if (data[matrixWidth * j + i].Equals(data[matrixWidth * j + (matrixWidth - 1 - i)]))
                {
                    simetry++;
                }
            }
        }
        if (elements == 0)
            return 0;
        return simetry / elements;
    }

    /// <summary>
    /// Gets the name of the evaluator.
    /// </summary>
    /// <returns> THe name of the evaluator. </returns>
    public override string GetName()
    {
        return "Horizontal 2D Simetry";
    }

    /// <summary>
    /// Returns the custom inspector GUI for this object.
    /// </summary>
    /// <returns>A <see cref="VisualElement"/> representing the custom inspector GUI.</returns>
    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }
}
