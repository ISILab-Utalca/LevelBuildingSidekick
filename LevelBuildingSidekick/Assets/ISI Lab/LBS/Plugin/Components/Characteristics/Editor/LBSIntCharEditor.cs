using ISILab.LBS.Characteristics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("Integer", typeof(LBSIntCharacteristic))]
    public class LBSIntCharEditor : LBSCustomEditor
    {
        public IntegerField input;

        public LBSIntCharEditor(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }
        public LBSIntCharEditor()
        {
            Add(CreateVisualElement());
        }

        public override void SetInfo(object obj)
        {
            this.target = obj;
            var target = obj as LBSIntCharacteristic;

            input.value = target.Value;
        }

        protected override VisualElement CreateVisualElement()
        {
            var cr = target as LBSIntCharacteristic;

            var ve = new VisualElement();

            input = new IntegerField("Value:");
            ve.Add(input);
            input.RegisterCallback<ChangeEvent<int>>(e =>
            {
                cr.Value = e.newValue;
            });
            return ve;
        }
    }
}