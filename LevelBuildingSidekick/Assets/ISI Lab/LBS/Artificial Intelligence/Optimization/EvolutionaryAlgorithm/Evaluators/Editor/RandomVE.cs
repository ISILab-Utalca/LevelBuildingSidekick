using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomVisualElement(typeof(RandomE))]
public class RandomVE : EvaluatorVE
{
    public RandomVE(IEvaluator evaluator) : base(evaluator)
    { }

    public override void Init()
    {
    }
}
