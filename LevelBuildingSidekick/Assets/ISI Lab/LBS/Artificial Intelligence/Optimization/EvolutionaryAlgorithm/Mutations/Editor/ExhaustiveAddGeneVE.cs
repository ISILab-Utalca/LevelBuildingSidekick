using ISILab.AI.Categorization;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ExhaustiveAddGeneVE : LBSCustomEditor
    {
        public ExhaustiveAddGeneVE(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var mut = paramTarget as ExhaustiveAddGene;
        }

        protected override VisualElement CreateVisualElement()
        {
            Add(new Label("Exhaustive Add Gene"));
            return this;
        }
    }
}