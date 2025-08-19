
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;



namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSToolbarToggle: ToolbarToggle
    {
        readonly string lbsClassName = "lbs-toolbar-toggle";

        #region Properties

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
                    else
                    {
                        _toggleIconElement.style.display = DisplayStyle.None;
                    }
                }
            }
        }
        
        #endregion

        private VisualElement _toggleIconElement;
        
        private VectorImage arrowIcon;
        private VectorImage toggleIcon;

        public LBSToolbarToggle() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(lbsClassName);
            
            _toggleIconElement = new VisualElement();
            _toggleIconElement.style.display = DisplayStyle.Flex;
            _toggleIconElement.AddToClassList("lbs-icon");
            this.Add(_toggleIconElement);
            _toggleIconElement.PlaceBehind(labelElement);
            
        }
        
        
    }
}

