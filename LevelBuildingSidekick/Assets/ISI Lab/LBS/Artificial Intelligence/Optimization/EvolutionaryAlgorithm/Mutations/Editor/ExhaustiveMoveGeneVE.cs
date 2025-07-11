using ISILab.AI.Categorization;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("ExhaustiveMoveGene", typeof(ExhaustiveMoveGene))]
    public class ExhaustiveMoveGeneVE : LBSCustomEditor
    {
        IntegerField range;

        public ExhaustiveMoveGeneVE(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var mut = paramTarget as ExhaustiveMoveGene;

            range.value = mut.Range;
            range.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 1)
                    range.SetValueWithoutNotify(1);
                mut.Range = e.newValue;
            });
        }

        protected override VisualElement CreateVisualElement()
        {
            range = new IntegerField();
            range.label = "Movement Range";
            Add(range);

            return this;
        }
    }
}