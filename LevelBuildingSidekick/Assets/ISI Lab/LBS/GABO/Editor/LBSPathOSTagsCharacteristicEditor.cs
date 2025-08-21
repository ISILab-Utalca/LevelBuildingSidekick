using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("PathOSTag Identifier", typeof(LBSPathOSTagsCharacteristic))]
    public class LBSPathOSTagsCharacteristicEditor : LBSCustomEditor
    {
        public DropdownField dropdownField;

        public LBSPathOSTagsCharacteristicEditor()
        {
            CreateVisualElement();
        }

        public LBSPathOSTagsCharacteristicEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            var potc = target as LBSPathOSTagsCharacteristic;
            this.target = target;
            var storage = LBSAssetsStorage.Instance;

            if (potc == null)
                return;

            var pathOSTags = storage.Get<PathOSTag>();
            dropdownField.choices = pathOSTags.Select(t => t.Label).ToList();

            if (potc.Value != null)
            {
                dropdownField.SetValueWithoutNotify(potc.Value.name);
            }

            dropdownField.RegisterValueChangedCallback(e =>
            {
                var tag = pathOSTags.Find(t => t.name == e.newValue);
                potc.Value = tag;
            });

        }

        protected override VisualElement CreateVisualElement()
        {
            dropdownField = new DropdownField("Value:");
            this.Add(dropdownField);

            return this;
        }
    }
}
