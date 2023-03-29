using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class EvaluatorVE : VisualElement
{
    protected IEvaluator evaluator;

    public EvaluatorVE()
    {

    }

    public EvaluatorVE(IEvaluator evaluator)
    {
        this.evaluator = evaluator;
    }

}
