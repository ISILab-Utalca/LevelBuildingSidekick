using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;

[System.Serializable]
public class StampPresenceEvaluator : IRangedEvaluator
{
    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

    public StampPresset stamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="StampPresenceEvaluator"/> class
    /// </summary>
    public StampPresenceEvaluator()
    {
        var t = Utility.DirectoryTools.GetScriptables<StampPresset>();
        stamp = t.First();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StampPresenceEvaluator"/> with the specified <see cref="StampPresset"/>
    /// </summary>
    /// <param name="stamp"></param>
    public StampPresenceEvaluator(StampPresset stamp)
    {
        this.stamp = stamp;
    }

    /// <summary>
    /// Evaluates the presence of the specified <see cref="StampPresset"/> in <see cref="IEvaluable"/>
    /// </summary>
    /// <param name="evaluable"></param>
    /// <returns> A value between <see cref="MinValue"/> and <see cref="MaxValue"/> </returns>
    public float Evaluate(IEvaluable evaluable)
    {
        float presence = 0;

        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;

        if (!stmc.stamps.Any(s => s.Label == stamp.Label))
        {
            return MinValue;
        }

        var index = stmc.stamps.FindIndex(s => s.Label == stamp.Label);

        var data = stmc.GetGenes<int>();
        foreach (var i in data)
        {
            if (index == i)
            {
                presence++;
            }
        }
        presence /= data.Length;

        return Mathf.Clamp(presence,MinValue,MaxValue);
    }

    /// <summary>
    /// Gets the name of the evaluator.
    /// </summary>
    /// <returns> The name of the evaluator </returns>
    public string GetName()
    {
        return "Stamp Presence";
    }

    /// <summary>
    /// Creates the GUI for the evaluator.
    /// </summary>
    /// <returns> A <see cref="VisualElement"/> containing the GUI </returns>
    public VisualElement CIGUI()
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
        of.RegisterValueChangedCallback((e) => stamp = e.newValue as StampPresset);

        content.Add(v2);
        content.Add(of);

        return content;
    }
}
