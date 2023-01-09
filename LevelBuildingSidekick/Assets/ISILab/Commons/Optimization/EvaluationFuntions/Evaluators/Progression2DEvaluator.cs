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

    /// <summary>
    /// Creates a new Progression2DEvaluator object.
    /// </summary>
    public Progression2DEvaluator()
    {
        var t = Utility.DirectoryTools.GetScriptables<StampPresset>();
        stamp = t.First();
    }

    /// <summary>
    /// Evaluates the given evaluable object.
    /// </summary>
    /// <param name="evaluable">The evaluable object to evaluate.</param>
    /// <returns>A float value representing the evaluation of the given object.</returns>
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


    /// <summary>
    /// Evaluate the progression of the given data array.
    /// </summary>
    /// <param name="stmc">The chromosome to evaluate.</param>
    /// <param name="id">The data array to calculate simetry for.</param>
    /// <param name="height">The height of the data array.</param>
    /// <returns>A float value representing the simetry of the given data array.</returns>
    public abstract float EvaluateProgression(StampTileMapChromosome stmc, int id, int height);

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
