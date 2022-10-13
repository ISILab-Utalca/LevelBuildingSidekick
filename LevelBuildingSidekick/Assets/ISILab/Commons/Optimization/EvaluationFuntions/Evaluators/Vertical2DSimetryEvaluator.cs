using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public class Vertical2DSimetryEvaluator : Simetry2DEvaluator
{
    public override float MaxValue => 1;
    
    public override float MinValue => 0;

    public Vertical2DSimetryEvaluator() : base() { }

    public override float CalculateSimetry(object[] data, int height)
    {
        float simetry = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth / 2; i++)
            {
                if (data[matrixWidth * j + i].Equals(data[matrixWidth * j + (matrixWidth - i)]))
                {
                    simetry++;
                }
            }
        }
        return simetry;
    }

    public override string GetName()
    {
        return "Vertical 2D simetry";
    }

    public override VisualElement CIGUI()
    {
        var ve = new VisualElement();
        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(v => {
            Debug.LogWarning("Falta implementar");
            //this.MinValue = v.newValue.x;
            //this.MaxValue = v.newValue.y;
        });
        ve.Add(v2);
        ve.Add(v2);
        return ve;
    }
}
