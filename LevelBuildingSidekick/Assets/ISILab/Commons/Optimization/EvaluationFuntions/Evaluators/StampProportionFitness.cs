using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

public class StampProportionFitness : IRangedEvaluator
{
    public StampPresset stamp1;
    public StampPresset stamp2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

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
        of1.RegisterCallback<ChangeEvent<StampPresset>>((e) => stamp1 = e.newValue);


        ObjectField of2 = new ObjectField("Stamp 2: ");
        of2.objectType = typeof(StampPresset);
        of2.value = stamp2;
        of2.RegisterCallback<ChangeEvent<StampPresset>>((e) => stamp2 = e.newValue);

        content.Add(v2);
        content.Add(of1);
        content.Add(of2);

        return content;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if(!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        var data = stmc.GetGenes<int>();

        var p1count = stmc.stamps.Select(s => s.Label == stamp1.Label).Count();
        var p2count = stmc.stamps.Select(s => s.Label == stamp2.Label).Count();

        if (p1count == 0 || p2count == 0)
        {
            if(p1count == 0 && p2count == 0)
            {
                return MaxValue;
            }

            return MinValue;
        }

        int counterP1 = 0;
        int counterP2 = 0;

        foreach (var id in data)
        {
            if(stamp1.Label == stmc.stamps[id].Label)
            {
                counterP1++;
            }
            if (stamp2.Label == stmc.stamps[id].Label)
            {
                counterP2++;
            }
        }

        var p = counterP1 / counterP2;

        return p > MaxValue ? MaxValue / p : p;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
