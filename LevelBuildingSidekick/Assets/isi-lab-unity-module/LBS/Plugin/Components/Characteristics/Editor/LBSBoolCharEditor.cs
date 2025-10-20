using ISILab.LBS.Characteristics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("Boolean", typeof(LBSBoolCharacteristic))]
    public class LBSBoolCharEditor : LBSCustomEditor
    {
        public Toggle toogle;

        public LBSBoolCharEditor(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var target = paramTarget as LBSBoolCharacteristic;

            toogle.value = target.Value;
        }

        protected override VisualElement CreateVisualElement()
        {
            var ve = new VisualElement();
            var target = this.target as LBSBoolCharacteristic;

            toogle = new Toggle("Value:");
            ve.Add(toogle);
            toogle.RegisterCallback<ChangeEvent<bool>>(e =>
            {
                target.Value = e.newValue;
            });

            return ve;
        }
    }
}