using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;



namespace ISILab.LBS.CustomComponents
{
    
    [UxmlElement]
    public partial class LBSToolbarMenu: ToolbarMenu
    {
        readonly string lbsClassName = "lbs-toolbar-menu";
        
        #region Properties
        [UxmlAttribute]
        public VectorImage ArrowIcon
        {
            get => arrowIcon;
            set
            {
                arrowIcon = value;
                if (_arrowElement != null)
                {
                    if (arrowIcon != null)
                    {
                        _arrowElement.style.backgroundImage = new StyleBackground(arrowIcon);
                        _arrowElement.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        _arrowElement.style.display = DisplayStyle.None;
                    }
                }
            }
        }

        [UxmlAttribute]
        public VectorImage ToggleIcon
        {
            get => toggleIcon;
            set
            {
                toggleIcon = value;
                if (_toggleIconElement != null)
                {
                    if (toggleIcon != null)
                    {
                        _toggleIconElement.style.backgroundImage = new StyleBackground(toggleIcon);
                        _toggleIconElement.style.display = DisplayStyle.Flex;
                    }
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
                if (iconColor != Color.white && _toggleIconElement != null)
                {
                    _toggleIconElement.style.unityBackgroundImageTintColor = new StyleColor(iconColor);
                }
            }
        }
        
        #endregion

        private VisualElement _arrowElement;
        private VisualElement _toggleIconElement;
        
        private VectorImage arrowIcon;
        private VectorImage toggleIcon;
        private Color iconColor = Color.white;
        
        public LBSToolbarMenu() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(lbsClassName);
            
            
            
            _arrowElement = this.Q<VisualElement>(classes: arrowUssClassName);
            if (arrowIcon != null)
            {
                _arrowElement.style.backgroundImage = new StyleBackground(arrowIcon);
                _arrowElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                _arrowElement.style.display = DisplayStyle.None;
            }
            
            _toggleIconElement = new VisualElement();
            _toggleIconElement.AddToClassList("lbs-icon");
            if (toggleIcon != null)
            {
                _toggleIconElement.style.backgroundImage = new StyleBackground(toggleIcon);
                _toggleIconElement.style.display = DisplayStyle.Flex; 
            }
            else
            {
                _toggleIconElement.style.backgroundImage = new StyleBackground(Macros.LBSAssetMacro.LoadPlaceholderTexture());
            }
            this.Add(_toggleIconElement);   
            
            
            TextElement textElement = this.Q<TextElement>(classes: textUssClassName);
            if (textElement != null)
            {
                _toggleIconElement.PlaceBehind(textElement);
            }

        }
    }
}



