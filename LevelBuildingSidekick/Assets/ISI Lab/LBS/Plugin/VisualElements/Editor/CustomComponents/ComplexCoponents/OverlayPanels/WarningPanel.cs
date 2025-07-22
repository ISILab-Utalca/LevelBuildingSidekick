using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class WarningPanel : VisualElement
    {
        
        public enum WarningType {Default, Log, Warning, Error }
        
        #region FACTORY
        
        // UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
        // {
        //     name = "Text",
        //     defaultValue = "...",
        // };

        // public IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        // {
        //     get
        //     {
        //         yield return new UxmlChildElementDescription(typeof(VisualElement));
        //     }
        // }
        
        #endregion

        #region FIELDS
        
        private string text = "";
        private WarningType currentWarningType = WarningType.Default;
        #endregion

        #region FIELDS VIEWS
        
        private Label labelVisualElement;
        private VisualElement iconVisualElement;
        private VisualElement externalBorder;
        
        #endregion

        #region PROPERTIES
        
        [UxmlAttribute]
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (labelVisualElement != null)
                {
                    labelVisualElement.text = value;
                }
            }
        }

        [UxmlAttribute]
        public WarningType CurrentWarningType
        {
            get => currentWarningType;
            set
            {
                currentWarningType = value;
            }
        }

        #endregion

        #region CONSTRUCTORS

        public WarningPanel() : this("Placeholder Text")
        {
            
        }


        public WarningPanel(string _text)
        {
            // Warning Panel UXML
            VisualTreeAsset visualTree = Macros.LBSAssetMacro.LoadAssetByGuid<VisualTreeAsset>("65ad21365d671594a85cf837da5d8c84"); 
            visualTree.CloneTree(this);

            this.labelVisualElement = this.Q<Label>("Text");
            this.iconVisualElement = this.Q<VisualElement>("Icon");
            
            Text = _text;
            labelVisualElement.text = this.Text;
        }
        
        #endregion
    }
}
