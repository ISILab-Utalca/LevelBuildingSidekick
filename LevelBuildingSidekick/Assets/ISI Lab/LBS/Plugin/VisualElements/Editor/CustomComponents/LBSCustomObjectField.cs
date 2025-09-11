using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomObjectField: ObjectField
    {
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSFieldClassName = "lbs-object-field";
        
        private VectorImage iconImage;
        private IconPosition iconPosition = IconPosition.Left;

        private VisualElement iconVisualElement;
        
        #region Properties

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
            
            if (iconImage != null)
            {
                iconVisualElement.style.backgroundImage = new StyleBackground(iconImage);
                SetIconPosition(iconPosition);
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
