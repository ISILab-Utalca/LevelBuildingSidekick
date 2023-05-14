
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using GeneticSharp.Domain.Chromosomes;

public class Resemblance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public float LocalMax => 0.75f;

    public float LocalMin => 0.25f;

    public ChromosomeBase reference;

    public float Evaluate(IOptimizable evaluable)
    {
        var ev = evaluable as ChromosomeBase;
        if (ev is null)
            return MinValue;
        if (ev.Length != reference.Length)
            return MinValue;

        float threshold = MaxValue - MinValue;
        int diff = 0;
        for(int i = 0; i < reference.Length; i++)
        {
            if(reference.GetGene(i) == null)
            {
                if(ev.GetGene(i) != null)
                {
                    diff++;
                    continue;
                }
                continue;
            }
            if (!reference.GetGene(i).Equals(ev.GetGene(i)))
                diff++;
        }

        float count = (reference.Length - ev.ImmutablesCount * 1f);

        var fit = MinValue + threshold * ((count - (diff*1f)) / count);

        return fit;
    }
}
