using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

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

    public VisualElement CIGUI()
    {
        var ve = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue,this.MaxValue);
        v2.RegisterValueChangedCallback(v => {
            Debug.LogWarning("Falta implementar");
            //this.MinValue = v.newValue.x;
            //this.MaxValue = v.newValue.y;
        });
        ve.Add(v2);

        var so = new ObjectField("Stamp reference");
        so.objectType = typeof(StampPresset);
        so.RegisterValueChangedCallback(v => {
            Debug.LogWarning("Falta implementar");
            //this.element = v.newValue;
        });
        ve.Add(so);
        return ve;
    }
}
