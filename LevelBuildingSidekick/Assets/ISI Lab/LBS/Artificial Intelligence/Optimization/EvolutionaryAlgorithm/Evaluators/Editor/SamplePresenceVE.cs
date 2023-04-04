using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[CustomVisualElement(typeof(SamplePresence))]
public class SamplePresenceVE : EvaluatorVE
{
    ObjectField objectField;

    public SamplePresenceVE(IEvaluator evaluator) : base(evaluator)
    {
        objectField = new ObjectField("Sample");
        objectField.objectType = typeof(Bundle);
        objectField.RegisterCallback<SerializedObjectChangeEvent> (
            (e) => {
                (evaluator as SamplePresence).Sample = e.changedObject;
        });
        Add(objectField);
    }

    public override void Init()
    {
    }
}
