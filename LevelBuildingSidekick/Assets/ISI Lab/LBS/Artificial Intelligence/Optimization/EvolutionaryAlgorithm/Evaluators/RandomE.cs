using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;

public class RandomE : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public float LocalMax => 0.75f;

    public float LocalMin => 0.25f;

    public float Evaluate(IOptimizable evaluable)
    {
        var r = RandomizationProvider.Current;
        return r.GetFloat();
    }
}
