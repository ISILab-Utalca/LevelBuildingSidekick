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
        
        [UxmlAttribute]
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (label != null)
                {
                    label.text = value;
                }
            }
        }
        #endregion

        #region CONSTRUCTORS
        public WarningPanel()
        {
            var visualTree = Macros.LBSAssetMacro.LoadAssetByGuid<VisualTreeAsset>("65ad21365d671594a85cf837da5d8c84"); //Warning Panel
            visualTree.CloneTree(this);

            this.label = this.Q<Label>("Text");
            label.text = this.Text;
        }
        #endregion
    }
}
