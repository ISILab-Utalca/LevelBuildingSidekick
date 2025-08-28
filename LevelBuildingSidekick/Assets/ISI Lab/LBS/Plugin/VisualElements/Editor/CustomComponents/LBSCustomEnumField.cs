using System;
using System.Drawing;
using System.Linq;
using ISILab.Macros;
using NUnit.Framework;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomEnumField: EnumField
    {
        
        #region constants
        
        public readonly static string LBSClassName = "lbs-field";
        public readonly static string LBSEnumFieldClass = ".lbs-enum-field";
        
        #endregion
        
        
        #region Parameters

        [UxmlAttribute]
        public int IconSize
        {
            get => iconSize;
            set
            {
                iconSize = value;
                if (iconVisualElement != null)
                {
                    iconVisualElement.style.width = value;
                    iconVisualElement.style.height = value;
                }
            }
        }

        [UxmlAttribute]
        public VectorImage IconImage
        {
            get => iconImage;
            set
            {
                iconImage = value;
                if (iconVisualElement != null && iconImage != null)
                {
                    iconVisualElement.style.backgroundImage = new StyleBackground(IconImage);
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
        
        private int iconSize = 16;
        private VectorImage iconImage;
        private Color iconColor = Color.white;
        private IconPosition iconPosition = IconPosition.None;
        
        
        #endregion
        
        private VisualElement iconVisualElement;

        public LBSCustomEnumField(): this("CustomEnumField"){}

        public LBSCustomEnumField(string _label) : base(_label)
        {
            style.alignItems = Align.Center;
            this.AddToClassList(LBSClassName);
            this.AddToClassList(LBSEnumFieldClass);
            iconVisualElement = new VisualElement();
            labelElement.AddToClassList("lbs-label");
            labelElement.RemoveFromClassList("unity-label");
            
            // input
            VisualElement inputButton = this.Q<VisualElement>(classes: inputUssClassName);
            inputButton.AddToClassList("lbs-enum-field-input");
            inputButton.AddToClassList("lbs-button");
            inputButton.RemoveFromClassList(inputUssClassName);
            
            
            // arrow
            VisualElement arrow = this.Q<VisualElement>(classes: arrowUssClassName);
            arrow.AddToClassList("lbs-arrow");
            arrow.RemoveFromClassList(arrowUssClassName);
            
            
            //styles
            iconVisualElement.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            iconVisualElement.style.width = IconSize;
            iconVisualElement.style.height = IconSize;
            
            iconVisualElement.AddToClassList("lbs-icon");
            this.Add(iconVisualElement);
            SetIconPosition(iconVisualElement, iconPosition);

            if (IconImage != null)
            {
                iconVisualElement.style.backgroundImage = new StyleBackground(IconImage);
                iconVisualElement.style.unityBackgroundImageTintColor = iconColor;
            }
            
            // RegisterCallback<AttachToPanelEvent>(e => { });
            // RegisterCallback<DetachFromPanelEvent>(e => { });
            
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

        public void Init(string firstValue)
        {

        }
        
    }
}
