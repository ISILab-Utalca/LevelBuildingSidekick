using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Fitness;
using System.Linq;

public class ResemblanceEvaluator : IRangedEvaluator
{
    public float MaxValue { get { return 1; } }
    public float MinValue { get { return 0; } }

    public object[] Base { get; set; }

    public float Evaluate(IEvaluable evaluable)
    {
        var data = evaluable.GetData<object[]>();
        int dist = 0;

        if(Base.Length != data.Length)
        {
            Debug.LogWarning("Sequences to compare are not of the same length");
            var length = Base.Length > data.Length ? Base.Length : data.Length;
            return Mathf.Abs(Base.Length - data.Length) / (1.0f * length);
        }

        for(int i = 0; i < data.Length; i++)
        {
            if(!data[i].Equals(Base[i]))
            {
                dist++;
            }
        }

        return dist / (1.0f * data.Length);
    }
}
