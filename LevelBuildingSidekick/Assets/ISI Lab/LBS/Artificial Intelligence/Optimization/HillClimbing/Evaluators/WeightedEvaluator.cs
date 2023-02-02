using System.Linq;
using System;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class WeightedEvaluator : IEvaluator
{
    Tuple<IEvaluator, float>[] evaluators;

    public WeightedEvaluator(params Tuple<IEvaluator, float>[] evaluators)
    {
        this.evaluators = evaluators;
    }

    public VisualElement CIGUI()
    {
        throw new NotImplementedException();
    }

    public float Evaluate(IOptimizable evaluable)
    {
        float totalWeight = 0;
        float fitness = 0;

        foreach(var t in evaluators)
        {
            totalWeight += t.Item2;
            fitness += t.Item1.Evaluate(evaluable);
        }

        return fitness/totalWeight;
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}