using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using System.Linq;

[System.Serializable]
public class ResemblanceEvaluator : IRangedEvaluator
{
    public float MaxValue { get { return 1; } }
    public float MinValue { get { return 0; } }

    public object[] Sample{ get; set; }

    public float Evaluate(IEvaluable evaluable)
    {
        var data = evaluable.GetDataSquence<object>();
        int dist = 0;

        if (Sample.Length != data.Length)
        {
            Debug.LogWarning("Sequences to compare are not of the same length - L1: " + Sample.Length + " - L2: " + data.Length);
            var length = Sample.Length > data.Length ? Sample.Length : data.Length;
            return Mathf.Abs(Sample.Length - data.Length) / (1.0f * length);
        }

        for (int i = 0; i < data.Length; i++)
        {
            if (!data[i].Equals(Sample[i]))
            {
                dist++;
            }
        }

        var n = 1 - (dist / (1.0f * data.Length));

        return Mathf.Clamp(n, MinValue, MaxValue);
    }

    public string GetName()
    {
        return "Resemblance";
    }
}
