using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public class Horizontal2DSimetry : Simetry2DEvaluator
{
    public Horizontal2DSimetry() : base() { }

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

    public override string GetName()
    {
        return "Horizontal 2D Simetry";
    }

    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }
}
