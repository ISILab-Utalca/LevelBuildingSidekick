using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace LBS.VisualElements
{
    public class SubPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SubPanel, UxmlTraits> { }

        private Label title;
        private Label extraLabel;
        private VisualElement contentPanel;

        public SubPanel()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("SubPanelUXML");
            visualTree.CloneTree(this);

            this.title = this.Q<Label>("Label");
            this.extraLabel = this.Q<Label>("ExtraLabel");
            this.contentPanel = this.Q<VisualElement>("Content");
        }

        public void SetValue(object obj, string title, string extraTitle = "")
        {
            ClearValue();
            this.title.text = title;
            this.extraLabel.text = extraTitle;

            var type = obj.GetType();
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                var t = field.FieldType;
                var n = field.Name;
                Debug.Log(n+": "+t);

                if(t == typeof(int))
                {
                    var x = new IntegerField(n);
                    x.RegisterCallback<ChangeEvent<int>>(v =>
                    {
                        var o = obj;
                        var f = field;
                        f.SetValue(o, v.newValue);
                    });
                }
                else if (t == typeof(float))
                {
                    var x = new FloatField(n);
                    x.RegisterCallback<ChangeEvent<float>>(v =>
                    {
                        var o = obj;
                        var f = field;
                        f.SetValue(o, v.newValue);
                    });
                }
            }
        }

        public void ClearValue()
        {
            this.contentPanel.Clear();
        }
    }
}