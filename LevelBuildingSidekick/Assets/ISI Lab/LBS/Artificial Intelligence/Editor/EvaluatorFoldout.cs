using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EvaluatorFoldout : VisualElement
{
    IEvaluator evaluator;

    public EvaluatorFoldout()
    {

    }

    public EvaluatorFoldout(IEvaluator evaluator)
    {
        this.evaluator = evaluator;
    }

}
