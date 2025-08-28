using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomDropdown: DropdownField
    {
        #region constants
            
            public static string LBSClassName = "lbs-field";
            public static string LBSEnumFieldClass = ".lbs-dropdown-field";
            
            #endregion  
            
            #region Attributes
            
            [UxmlAttribute]
            public VectorImage IconImage
            {
                get => iconImage;
                set
                {
                    iconImage = value;
                    if (iconVisualElement != null)
                    {
                        if (iconImage == null)
                        {
                            iconVisualElement.style.display = DisplayStyle.None;
                            return;
                        }
                        iconVisualElement.style.display = DisplayStyle.Flex;
                        iconVisualElement.style.backgroundImage = new StyleBackground(iconImage);
                    }
                }
            }

            [UxmlAttribute]
            public Color IconColor
            {
                get => iconColor;
                set
                {
                    iconColor = value;
                    if (iconVisualElement != null)
                    {
                        iconVisualElement.style.unityBackgroundImageTintColor = value;
                    }
                }

            }
            
            [UxmlAttribute]
            public IconPosition IconPosition
            {
                get => iconPosition;
                set
                {
                    iconPosition = value;
                
                    if (iconVisualElement != null)
                    {
                        SetIconPosition(iconVisualElement, iconPosition);
                    }
                }
            }

            #endregion
            
            
            #region Fields
            
            private VectorImage iconImage;
            private Color iconColor = Color.white;
            private IconPosition iconPosition = IconPosition.Left;


            private VisualElement iconVisualElement;
            
            #endregion
            
            
            
            public LBSCustomDropdown() : base()
            {
                iconVisualElement = SetupIconVisualElement();
                this.Add(iconVisualElement);
                SetIconPosition(iconVisualElement,iconPosition);
                
            }

            VisualElement SetupIconVisualElement()
            {
                VisualElement _button = new VisualElement();
                _button.AddToClassList(LBSCustomStyle.LBS_ICON);
                
                if (iconImage != null)
                {
                    _button.style.backgroundImage = new StyleBackground(iconImage);
                }
                else
                {
                    _button.style.display = DisplayStyle.None;
                }
                
                return _button;
            }
            
            void SetIconPosition(VisualElement _iconVe, IconPosition _iconPosition)
            {
                switch (_iconPosition)
                {
                    case IconPosition.Left:
                        _iconVe.style.display = DisplayStyle.Flex;
                        _iconVe.SendToBack();
                        break;
                    case IconPosition.None:
                        _iconVe.style.display = DisplayStyle.None;
                        break;
                    case IconPosition.Right:
                        _iconVe.style.display = DisplayStyle.Flex;
                        _iconVe.BringToFront();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
            
            
    }
}
