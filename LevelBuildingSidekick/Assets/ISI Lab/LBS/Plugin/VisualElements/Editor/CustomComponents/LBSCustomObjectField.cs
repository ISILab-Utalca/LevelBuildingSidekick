using System;
using System.Drawing;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomObjectField: ObjectField
    {
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSFieldClassName = "lbs-object-field";
        
        private VectorImage vectorIcon;
        private IconPosition iconPosition = IconPosition.Left;

        private VisualElement iconVisualElement;
        
        #region Properties

        [UxmlAttribute]
        public VectorImage VectorIcon
        {
            get => vectorIcon;
            set
            {
                vectorIcon = value;
                if (iconVisualElement != null)
                {
                    if (vectorIcon == null)
                    {
                        iconVisualElement.style.display = DisplayStyle.None;
                        return;
                    }
                    iconVisualElement.style.display = DisplayStyle.Flex;
                    iconVisualElement.style.backgroundImage = new StyleBackground(vectorIcon);
                }
            }
        }

        [UxmlAttribute]
        public IconPosition IconSide
        {
            get => iconPosition;
            set
            {
                iconPosition = value;
                
                if (iconVisualElement != null)
                {
                    SetIconPosition(iconPosition);
                }
            }
        }

        #endregion
        
        public LBSCustomObjectField() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
            AddToClassList(LBSFieldClassName);
            
            iconVisualElement = new VisualElement();
            iconVisualElement.AddToClassList(LBSCustomStyle.LBS_ICON);
            this.Add(iconVisualElement);
            if (vectorIcon != null)
            {
                iconVisualElement.style.backgroundImage = new StyleBackground(vectorIcon);
                switch (iconPosition)
                {
                    case IconPosition.Left:
                        iconVisualElement.SendToBack();
                        break;
                    case IconPosition.Right:
                        iconVisualElement.SendToBack();
                        break;
                    case IconPosition.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                iconVisualElement.style.display = DisplayStyle.None;
            }
        }

        void SetIconPosition(IconPosition _iconPosition)
        {
            switch (_iconPosition)
            {
                case IconPosition.Left:
                    iconVisualElement.style.display = DisplayStyle.Flex;
                    iconVisualElement.SendToBack();
                    break;
                case IconPosition.None:
                    iconVisualElement.style.display = DisplayStyle.None;
                    break;
                case IconPosition.Right:
                    iconVisualElement.style.display = DisplayStyle.Flex;
                    iconVisualElement.BringToFront();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}
