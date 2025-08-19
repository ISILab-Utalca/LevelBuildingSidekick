
using System;
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

        [UxmlAttribute]
        public Boolean HideToggle
        {
            get => hideToggle;
            set
            {
                hideToggle = value;
                if (inputVisualElement != null)
                {
                    if (hideToggle)
                    {
                        inputVisualElement.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        inputVisualElement.style.display = DisplayStyle.Flex;
                    }
                }
            }
        }

        #endregion

        private VisualElement _toggleIconElement;
        private VisualElement inputVisualElement;
        
        private VectorImage toggleIcon;
        private bool hideToggle = true;

        public LBSToolbarToggle() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(lbsClassName);
            
            _toggleIconElement = new VisualElement();
            _toggleIconElement.AddToClassList("lbs-icon");
            Add(_toggleIconElement);
            if (toggleIcon != null)
            {
                _toggleIconElement.style.backgroundImage = new StyleBackground(toggleIcon);
                _toggleIconElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                _toggleIconElement.style.backgroundImage = null;
                _toggleIconElement.style.display = DisplayStyle.None;
            }
            
            inputVisualElement = this.Q<VisualElement>(classes: inputUssClassName);

            if (hideToggle)
            {
                inputVisualElement.style.display = DisplayStyle.None;
            }
            //_toggleIconElement.PlaceBehind(labelElement);
            
        }
        
        
    }
}

