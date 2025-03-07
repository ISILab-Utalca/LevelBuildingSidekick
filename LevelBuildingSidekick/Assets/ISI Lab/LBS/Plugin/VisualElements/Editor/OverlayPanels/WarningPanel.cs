using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class WarningPanel : VisualElement
    {
        #region FACTORY
        //public new class UxmlFactory : UxmlFactory<WarningPanel, UxmlTraits> { }

        UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
        {
            name = "Text",
            defaultValue = "...",
        };

        public IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get
            {
                yield return new UxmlChildElementDescription(typeof(VisualElement));
            }
        }

        public void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            Init(ve, bag, cc);
            var panel = ve as WarningPanel;

            panel.Text = m_Text.GetValueFromBag(bag, cc);
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
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("WarningPanel");
            visualTree.CloneTree(this);

            this.label = this.Q<Label>("Text");
        }
        #endregion
    }
}
