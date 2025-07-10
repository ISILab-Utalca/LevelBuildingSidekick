using System.Linq;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomEnumField: EnumField
    {
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

        #endregion
        
        
        
        private int iconSize = 16;
        private VectorImage iconImage;
        
        private VisualElement m_Icon;

        public LBSCustomEnumField(): this("CustomEnumField"){}

        public LBSCustomEnumField(string _label) : base(_label)
        {
            style.alignItems = Align.Center;
            
            m_Icon = new VisualElement();
            
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
            }
            
            // RegisterCallback<AttachToPanelEvent>(e => { });
            // RegisterCallback<DetachFromPanelEvent>(e => { });
            
        }
        
    }
}
