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
        for (int j = 0; j < height / 2; j++)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                if (data[(matrixWidth * j) + i].Equals(data[(height - 1 - j)*matrixWidth + i]))
                {
                    simetry++;
                }
            }
        }
        return simetry;
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