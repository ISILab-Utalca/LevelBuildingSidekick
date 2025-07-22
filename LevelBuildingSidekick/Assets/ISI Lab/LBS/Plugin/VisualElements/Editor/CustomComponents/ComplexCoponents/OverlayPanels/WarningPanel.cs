using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class WarningPanel : VisualElement
    {
        
        public enum WarningType {Default, Log, Warning, Error, Success }
        
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

        
        private static Dictionary<WarningType, string> iconGuids = new()
        {
            { WarningType.Default, "3ceecc8c3279f3b45b9a1a2efad911a6" },
            { WarningType.Log , "8c0952dcbc9d49f4198ce33fdf7b4df5"},
            { WarningType.Warning, "5549d02f87d9642469d0336544f4cb88" },
            { WarningType.Error, "7bdf2adeb17673349abf65c6f8f0f411" },
            { WarningType.Success, "bda9551ce6aa6ee4fa2c8b46e36499cd" }
        };
        
        
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
                if (iconVisualElement != null && externalBorder != null)
                {
                    Color color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                    
                    switch (currentWarningType)
                    {
                        case WarningType.Default:
                            externalBorder.style.backgroundColor = color;
                            Color borderColor = Color.white;
                            iconVisualElement.style.unityBackgroundImageTintColor = borderColor;
                            borderColor.a = 0.3f;
                            externalBorder.style.borderBottomColor = borderColor;
                            externalBorder.style.borderTopColor = borderColor;
                            externalBorder.style.borderLeftColor = borderColor;
                            externalBorder.style.borderRightColor = borderColor;
                            
                            break;
                        case WarningType.Log:
                            color = LBSSettings.Instance.view.calloutColor;
                            break;  
                        case WarningType.Warning:
                            color = LBSSettings.Instance.view.warningColor;
                            break;
                        case WarningType.Error:
                            color = LBSSettings.Instance.view.errorColor;
                            break;
                        case WarningType.Success:
                            color = LBSSettings.Instance.view.successColor;
                            break;
                    }
                    
                    if (CurrentWarningType != WarningType.Default)
                    {
                        iconVisualElement.style.unityBackgroundImageTintColor = color;
                        externalBorder.style.borderBottomColor = color;
                        externalBorder.style.borderTopColor = color;
                        externalBorder.style.borderLeftColor = color;
                        externalBorder.style.borderRightColor = color;
                        color.a = 0.1f;
                        externalBorder.style.backgroundColor = color;
                    }

                    iconVisualElement.style.backgroundImage = new StyleBackground(
                        Macros.LBSAssetMacro.LoadAssetByGuid<VectorImage>(
                            iconGuids[currentWarningType]
                        ));
                }
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
            
            this.externalBorder = this.Q<VisualElement>("ExternalBorder");
            this.labelVisualElement = this.Q<Label>("Text");
            this.iconVisualElement = this.Q<VisualElement>("Icon");
            
            Text = _text;
            labelVisualElement.text = this.Text;
        }
        
        #endregion
    }
}
