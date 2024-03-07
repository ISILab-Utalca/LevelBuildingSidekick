using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Connections group", typeof(LBSDirectionedGroup))]
    public class LBSDirectionGroupEditor : LBSCustomEditor
    {
        public VisualElement content;

        public LBSDirectionGroupEditor()
        {

        }

        public LBSDirectionGroupEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);

        }

        public override void SetInfo(object obj)
        {
            this.target = obj;
            var target = obj as LBSDirectionedGroup;

            if (target == null)
                return;

            target._Update();

            content = new VisualElement();
            Add(content);
            var weights = target.Weights;

            // Show warning if there are no child bundles to add weights
            if(weights.Count <= 0)
            {
                var wp = new WarningPanel();
                wp.label.text = "This feature adds weights to the child bundles, " +
                    "make sure to have child bundles for this feature to work.";
                content.Add(wp);
                return;
            }

            // Intance the weights of the child bundles
            for (int i = 0; i < weights.Count; i++)
            {
                var current = weights[i];
                var box = new VisualElement();
                content.Add(box);

                box.Add(new Label(current.target.name));

                var slider = new Slider();
                slider.showInputField = true;
                box.Add(slider);
                slider.lowValue = 0;
                slider.highValue = 1;
                slider.value = current.weigth;
                slider.RegisterValueChangedCallback( evt =>
                {
                    current.weigth = evt.newValue;
                });
            }
        }

        protected override VisualElement CreateVisualElement()
        {
            var target = this.target as LBSDirectionedGroup;

            content = new VisualElement();

            return this;
        }
    }
}