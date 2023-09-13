using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
//using UnityEditor.UIElements;
using System.Linq;
using System;
using GeneticSharp.Domain.Chromosomes;

[System.Serializable]
public class SamplePresence : IRangedEvaluator
{ 
    public float MaxValue => 1;

    public float MinValue => 0;

    private object sample;
    public object Sample
    {
        get => sample;
        set => sample = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SamplePresence"/> class
    /// </summary>
    public SamplePresence()
    {
        Sample = default;
    }

    public SamplePresence(object sample)
    {
        Sample = sample;
    }

    /// <summary>
    /// Evaluates the presence of the specified <see cref="StampPresset"/> in <see cref="IOptimizable"/>
    /// </summary>
    /// <param name="evaluable"></param>
    /// <returns> A value between <see cref="MinValue"/> and <see cref="MaxValue"/> </returns>
    public float Evaluate(IOptimizable evaluable)
    {

        if (sample == null)
            return MinValue;

        if (!(evaluable is ChromosomeBase))
        {
            return MinValue;
        }

        var ev = evaluable as ChromosomeBase;

        float presence = ev.GetGenes().Count(e => e != null && e.Equals(sample));

        presence /= ev.Length*1f;

        return MinValue + ((MaxValue - MinValue) * presence);
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }
}
