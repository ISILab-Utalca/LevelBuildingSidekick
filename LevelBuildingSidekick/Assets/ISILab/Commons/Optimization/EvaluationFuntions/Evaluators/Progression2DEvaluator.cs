using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

public abstract class Progression2DEvaluator : IRangedEvaluator
{

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

    public StampPresset stamp;

    public Progression2DEvaluator()
    {
        var t = Utility.DirectoryTools.GetScriptables<StampPresset>();
        stamp = t.First();
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        var id = stmc.stamps.FindIndex(s => s.Label == stamp.Label);
        var height = stmc.Length / stmc.MatrixWidth;

        return Mathf.Clamp(EvaluateProgression(stmc, id, height), MinValue, MaxValue);
    }

    public abstract float EvaluateProgression(StampTileMapChromosome stmc, int id, int height);

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

        ObjectField of = new ObjectField("Stamp: ");
        of.objectType = typeof(StampPresset);
        of.value = stamp;
        of.RegisterValueChangedCallback((e) =>
        {
            if(e.newValue is StampPresset)
            {
                stamp = e.newValue as StampPresset;
            }
        });

        content.Add(v2);
        content.Add(of);

        return content;
    }
}
