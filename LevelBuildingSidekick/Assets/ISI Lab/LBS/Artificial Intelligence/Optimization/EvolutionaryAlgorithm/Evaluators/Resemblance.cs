
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using GeneticSharp.Domain.Chromosomes;

public class Resemblance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public IChromosome reference;

    public float Evaluate(IOptimizable evaluable)
    {
        var ev = evaluable as IChromosome;
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

        var fit = MinValue + threshold * (((reference.Length*1f) - (diff*1f)) / (reference.Length*1f));

        return fit;
    }
}
