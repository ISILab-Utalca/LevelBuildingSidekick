using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

public class ElementPresenceEvaluator : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public object element;

    ElementPresenceEvaluator(object element)
    {
        this.element = element;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        var data = evaluable.GetDataSquence<object>();
        int presence = 0;
        foreach(var obj in data)
        {
            if(obj.Equals(element))
            {
                presence++;
            }
        }
        return Mathf.Clamp(presence,MinValue,MaxValue);
    }
}
