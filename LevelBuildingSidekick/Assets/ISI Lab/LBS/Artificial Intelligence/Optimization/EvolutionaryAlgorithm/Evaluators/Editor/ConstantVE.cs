using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomVisualElement(typeof(ConstantE))]
public class ConstantVE : EvaluatorVE
{
    public FloatField field;
    public ConstantVE(IEvaluator evaluator) : base(evaluator)
    {
        field = new FloatField("Value");
        field.value = (evaluator as ConstantE).val;
        field.RegisterValueChangedCallback((evt) => (evaluator as ConstantE).val = evt.newValue);
        this.Add(field);
    }

    public override void Init()
    {
    }
}
