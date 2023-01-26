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


    /// <summary>
    /// Evaluates the given evaluable object.
    /// </summary>
    /// <param name="evaluable">The evaluable object to evaluate.</param>
    /// <returns>A float value representing the evaluation of the given object.</returns>
    /// <exception cref="FitnessException">Thrown if the evaluable object is not an instance of ITileMap.</exception>
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


    /// <summary>
    /// Calculates the simetry of the given data array.
    /// </summary>
    /// <param name="data">The data array to calculate simetry for.</param>
    /// <param name="height">The height of the data array.</param>
    /// <returns>A float value representing the simetry of the given data array.</returns>
    public abstract float CalculateSimetry(object[] data, int height);

    public abstract string GetName();

    /// <summary>
    /// Creates a visual element for the current object.
    /// </summary>
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

    public float EvaluateH<u>(IEvaluable evaluable, u Heu)
    {
        throw new System.NotImplementedException();
    }
}
