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
        
        
        #region Attributes

        [UxmlAttribute]
        public int IconSize
        {
            get => iconSize;
            set
            {
                iconSize = value;
                if (m_Icon != null)
                {
                    m_Icon.style.width = value;
                    m_Icon.style.height = value;
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
                if (m_Icon != null && iconImage != null)
                {
                    m_Icon.style.backgroundImage = new StyleBackground(IconImage);
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
                if (m_Icon != null)
                {
                    m_Icon.style.unityBackgroundImageTintColor = value;
                }
            }
        }

        [UxmlAttribute]
        public IconPosition IconPosition
        {
            get => iconPositon;
            set
            {
                throw new NotImplementedException("iconPosition not implemented yet");
            }
        }

        #endregion
        
        
        #region Fields
        
        private int iconSize = 16;
        private VectorImage iconImage;
        private Color iconColor = Color.white;
        private IconPosition iconPositon = IconPosition.None;
        
        #endregion
        
        private VisualElement m_Icon;

        public LBSCustomEnumField(): this("CustomEnumField"){}

        public LBSCustomEnumField(string _label) : base(_label)
        {
            style.alignItems = Align.Center;
            this.AddToClassList(LBSClassName);
            this.AddToClassList(LBSEnumFieldClass);
            m_Icon = new VisualElement();
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
            m_Icon.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            m_Icon.style.width = IconSize;
            m_Icon.style.height = IconSize;
            
            m_Icon.AddToClassList("lbs-icon");
            this.Add(m_Icon);
            m_Icon.PlaceBehind(this.Children().First());

            if (IconImage != null)
            {
                m_Icon.style.backgroundImage = new StyleBackground(IconImage);
                m_Icon.style.unityBackgroundImageTintColor = iconColor;
            }
            
            // RegisterCallback<AttachToPanelEvent>(e => { });
            // RegisterCallback<DetachFromPanelEvent>(e => { });
            
        }

        public void Init(string firstValue)
        {

        }
        
    }
}
