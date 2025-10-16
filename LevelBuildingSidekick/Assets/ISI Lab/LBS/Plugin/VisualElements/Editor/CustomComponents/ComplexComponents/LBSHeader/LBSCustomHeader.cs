using ISILab.LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomHeader: VisualElement
    {
        
        internal static readonly BindingId iconImageProperty = (BindingId) nameof (iconImage);
        
        #region Parameters

        [UxmlAttribute]
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (labelVisualElement != null)
                {
                    labelVisualElement.text = text;
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
                if (iconImage == null)
                {
                    return;
                }
                
                if (iconVisualElement != null)
                {
                    iconVisualElement.vectorImage = iconImage;
                    iconVisualElement.style.display = DisplayStyle.Flex;
                }
                else
                {
                    iconVisualElement.style.display = DisplayStyle.None;
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
                    iconVisualElement.tintColor = value;
                }
            }
            
        }
        

        #endregion
        
        private string text = "";
        private VectorImage iconImage;
        
        
        Label labelVisualElement;
        LBSCustomImage iconVisualElement;
        private VisualElement iconContainer;
        private Color iconColor = Color.white;

        public LBSCustomHeader(): this("LBS Sample Header")
        {
            
        }

        public LBSCustomHeader(string _text = "Placeholder")
        {
            this.AddToClassList("lbs-header");
            VisualTreeAsset visualTree = Macros.LBSAssetMacro.LoadAssetByGuid<VisualTreeAsset>("4b6c101d1038601419b07d23e8a28d3b"); 
            visualTree.CloneTree(this);
            //this.styleSheets =    
            iconVisualElement = this.Q<LBSCustomImage>("HeaderIcon");
            
            labelVisualElement = this.Q<Label>("HeaderLabel");
            
            Text = _text;
            labelVisualElement.text = this.Text;
            
        }
            
    }
}

