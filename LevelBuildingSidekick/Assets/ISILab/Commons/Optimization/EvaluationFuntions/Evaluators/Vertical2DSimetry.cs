using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public class Vertical2DSimetry : Simetry2DEvaluator
{
    public Vertical2DSimetry() : base() { }

    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth / 2; i++)
            {
                if (data[matrixWidth * j + i].Equals(data[matrixWidth * j + (matrixWidth - 1 - i)]))
                {
                    simetry++;
                }
            }
        }
        return simetry;
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
