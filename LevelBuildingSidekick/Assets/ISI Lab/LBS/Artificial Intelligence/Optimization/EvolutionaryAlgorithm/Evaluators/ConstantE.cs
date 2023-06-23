using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;

public class ConstantE
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public float val = 0;

    public float Evaluate(IOptimizable evaluable)
    {
        return val;
    }
}
