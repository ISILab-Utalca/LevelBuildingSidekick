using Commons.Optimization.Evaluator;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public abstract class EvaluatorVE : VisualElement
{
    protected IEvaluator evaluator;
    public IEvaluator Evaluator => evaluator;
    protected LBSLayer layer;

    public EvaluatorVE()
    {

    }

    public EvaluatorVE(IEvaluator evaluator)
    {
        this.evaluator = evaluator;
    }

    public virtual void SetLayer(LBSLayer layer)
    {
        this.layer = layer;
    }

    public abstract void Init();
}
