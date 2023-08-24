using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class WarningPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<WarningPanel, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "Text",
                defaultValue = "...",
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var panel = ve as WarningPanel;

                panel.Text = m_Text.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        #region FIELDS
        public string text = "";
        #endregion

        #region FIELDS VIEWS
        public Label label;
        #endregion

        #region PROPERTIES
        public string Text
        {
            get => text;
            set
            {
                text = value;
                label.text = value;
            }
        }
        #endregion

        #region CONSTRUCTORS
        public WarningPanel()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("WarningPanel");
            visualTree.CloneTree(this);

            this.label = this.Q<Label>("Text");
        }
        #endregion
    }
}
