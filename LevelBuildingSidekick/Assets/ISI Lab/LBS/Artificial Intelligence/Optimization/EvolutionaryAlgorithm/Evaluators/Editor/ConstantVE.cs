using Commons.Optimization.Evaluator;
using ISILab.AI.Categorization;
using ISILab.LBS.AI.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ConstantVE : EvaluatorVE
    {
        public FloatField field;
        public ConstantVE(IEvaluator evaluator) : base(evaluator)
        {
            field = new FloatField("Value");
            field.value = (evaluator as ConstantE).val;
            field.RegisterValueChangedCallback((evt) => (evaluator as ConstantE).val = evt.newValue);
            Add(field);
        }

        public override void Init() // TODO: Implement ConstantVE.Init
        {
        }
    }
}