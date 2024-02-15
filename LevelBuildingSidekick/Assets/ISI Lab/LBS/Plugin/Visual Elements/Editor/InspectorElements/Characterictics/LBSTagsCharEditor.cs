using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("Tag identifier", typeof(LBSTagsCharacteristic))]
    public class LBSTagsCharEditor : LBSCustomEditor
    {
        public DropdownField dropdownField;

        public LBSTagsCharEditor()
        {
            Add(CreateVisualElement());
        }

        public LBSTagsCharEditor(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            this.target = target;
            var tc = target as LBSTagsCharacteristic;

            dropdownField.value = tc?.Value?.Label;
        }

        protected override VisualElement CreateVisualElement()
        {
            var ve = new VisualElement();
            var storage = LBSAssetsStorage.Instance;
            var target = this.target as LBSTagsCharacteristic;

            dropdownField = new DropdownField("Value:");
            ve.Add(dropdownField);
            var tags = storage.Get<LBSIdentifier>();
            dropdownField.choices = tags.Select(t => t.Label).ToList();
            dropdownField.RegisterCallback<ChangeEvent<string>>(e =>
            {
                (this.target as LBSTagsCharacteristic).Value = tags.Find(t => t.Label == e.newValue);
            });
            return ve;
        }
    }
}