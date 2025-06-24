using ISILab.AI.Categorization;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ExhaustiveSwapGeneVE : LBSCustomEditor
    {

        public ExhaustiveSwapGeneVE(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var mut = paramTarget as ExhaustiveSwapGene;
        }

        protected override VisualElement CreateVisualElement()
        {
            Add(new Label("Exhaustive Swap Gene"));
            return this;
        }
    }
}