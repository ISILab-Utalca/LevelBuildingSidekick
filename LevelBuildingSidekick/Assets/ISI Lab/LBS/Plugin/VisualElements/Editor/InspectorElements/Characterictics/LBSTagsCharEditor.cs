using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
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
            CreateVisualElement();
        }

        public LBSTagsCharEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            var tc = target as LBSTagsCharacteristic;
            this.target = target;
            var storage = LBSAssetsStorage.Instance;


            if (tc == null)
                return;

            //Debug.Log(tc.Value);

            var tags = storage.Get<LBSTag>();
            dropdownField.choices = tags.Select(t => t.Label).ToList();

            if (tc.Value != null)
            {
                dropdownField.SetValueWithoutNotify(tc.Value.name);
            }
            
            dropdownField.RegisterValueChangedCallback(e =>
            {
                var tag = tags.Find(t => t.name == e.newValue);
                tc.Value = tag;
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