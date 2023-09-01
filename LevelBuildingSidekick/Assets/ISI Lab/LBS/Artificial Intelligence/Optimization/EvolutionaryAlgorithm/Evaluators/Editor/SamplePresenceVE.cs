using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using LBS.Bundles;

[CustomVisualElement(typeof(SamplePresence))]
public class SamplePresenceVE : EvaluatorVE
{
    ObjectField objectField;


    public SamplePresenceVE(IEvaluator evaluator) : base(evaluator)
    {
        objectField = new ObjectField("Sample");
        objectField.objectType = typeof(Bundle);
        objectField.RegisterValueChangedCallback(e => (evaluator as SamplePresence).Sample = e.newValue);
        Add(objectField);

    }

    public override void Init()
    {
    }
}
