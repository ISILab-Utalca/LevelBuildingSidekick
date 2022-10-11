using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;

[System.Serializable]
public class StampPresenceEvaluator : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public StampData element; 
    //public StampPresset element; 

    public StampPresenceEvaluator()
    {
        this.element = null;
    }

    public StampPresenceEvaluator(StampData element)
    {
        this.element = element;
    }


    public float Evaluate(IEvaluable evaluable)
    {
        int presence = 0;

        if (!(evaluable is StampTileMapChromosome))
        {
             
        }

        var c = evaluable as StampTileMapChromosome;

        if(!c.stamps.Contains(element))
        {
            return 0;
        }

        var index = c.stamps.FindIndex(e => e.Equals(element));

        var data = c.GetDataSquence<int>();
        foreach (var i in data)
        {
            if(index == i)
            {
                presence++;
            }
        }
        return Mathf.Clamp(presence,MinValue,MaxValue);
    }

    public string GetName()
    {
        return "Stamp presence";
    }
}
