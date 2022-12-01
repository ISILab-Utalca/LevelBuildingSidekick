using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;

public class StampProportionFitness : IRangedEvaluator
{
    public StampPresset stamp1;
    public StampPresset stamp2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;
     
    public StampProportionFitness()
    {
        var t = Utility.DirectoryTools.GetScriptables<StampPresset>();
        stamp1 = t.First();
        stamp2 = t.Last();
    }

    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        ObjectField of1 = new ObjectField("Stamp 1: ");
        of1.objectType = typeof(StampPresset);
        of1.value = stamp1;
        of1.RegisterValueChangedCallback(e => stamp1 = e.newValue as StampPresset);


        ObjectField of2 = new ObjectField("Stamp 2: ");
        of2.objectType = typeof(StampPresset);
        of2.value = stamp2;
        of2.RegisterValueChangedCallback(e => stamp2 = e.newValue as StampPresset);

        content.Add(v2);
        content.Add(of1);
        content.Add(of2);

        return content;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        var data = stmc.GetGenes<int>();

        if (!stmc.stamps.Any(s => s.Label == stamp1.Label))
            return MinValue;
        if (!stmc.stamps.Any(s => s.Label == stamp2.Label))
            return MinValue;

        float counterP1 = 0;
        float counterP2 = 0;

        foreach (var id in data)
        {
            if (id == -1)
                continue;
            var label = stmc.stamps[id].Label;
            if (stamp1.Label == label)
            {
                counterP1++;
            }
            else if (stamp2.Label == label)
            {
                counterP2++;
            }
        }

        if (counterP1 == 0 || counterP2 == 0)
        {
            return MinValue;// Temporal Fix, Should be changed
            //return counterP1 != counterP2 ? MinValue : MaxValue;
        }

        float p = counterP1 / counterP2;

        return p > MaxValue ? MaxValue*1f / p : p;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
