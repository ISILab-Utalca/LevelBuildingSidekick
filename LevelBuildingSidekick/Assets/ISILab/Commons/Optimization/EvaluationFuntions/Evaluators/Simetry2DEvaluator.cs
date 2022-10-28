using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class Simetry2DEvaluator : IRangedEvaluator
{
    float min = 0;
    float max = 1;
    public float MaxValue => max;
    public float MinValue => min;

    internal int matrixWidth;

    public Simetry2DEvaluator()
    {
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if(!(evaluable is ITileMap))
        {
            throw new FitnessException("evaluable must be ITileMap");
        }

        matrixWidth = (evaluable as ITileMap).MatrixWidth;

        var data = evaluable.GetDataSquence<object>();
        int height = data.Length / matrixWidth;

        float simetry = CalculateSimetry(data, height);

        return Mathf.Clamp(simetry, MinValue, MaxValue);
    }

    public abstract float CalculateSimetry(object[] data, int height);

    public abstract string GetName();

    public virtual VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        content.Add(v2);

        return content;
    }
}
