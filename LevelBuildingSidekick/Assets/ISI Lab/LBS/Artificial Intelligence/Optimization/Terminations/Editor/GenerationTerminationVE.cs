using ISILab.AI.Optimization.Terminations;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("GenerationNumberTermination", typeof(GenerationNumberTermination))]
    public class GenerationTerminationVE : LBSCustomEditor
    {
        IntegerField generations;

        public GenerationTerminationVE(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var term = paramTarget as GenerationNumberTermination;
            generations.value = term.ExpectedGenerationNumber;
            generations.RegisterValueChangedCallback(evt => term.ExpectedGenerationNumber = evt.newValue);
        }

        protected override VisualElement CreateVisualElement()
        {
            var term = target as GenerationNumberTermination;
            var ve = new VisualElement();

            generations = new IntegerField();
            generations.label = "Generations Number";

            ve.Add(generations);

            return ve;
        }
    }
}