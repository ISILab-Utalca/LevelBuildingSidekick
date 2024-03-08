using Commons.Optimization.Evaluator;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using LBS.Bundles;
using ISILab.LBS.AI.VisualElements;
using ISILab.LBS.Internal;
using ISILab.AI.Categorization;

namespace ISILab.LBS.VisualElements
{
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
}