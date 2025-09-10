using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSPanelTextIcon : VisualElement
    {
        #region FIELDS
        private readonly Label _label;
        private readonly VisualElement _iconElement;
        private readonly VisualElement _root;
        
        private string _text;
        private VectorImage _iconImage;
        private float _opacity = 1f;
        private FlexDirection _direction = FlexDirection.Row; // default
        #endregion

        #region PROPERTIES
        [UxmlAttribute]
        public string text
        {
            get => _text;
            set
            {
                _text = value;
                if (_label != null)
                    _label.text = _text ?? string.Empty;
            }
        }

        [UxmlAttribute]
        public VectorImage icon
        {
            get => _iconImage;
            set
            {
                _iconImage = value;
                if (_iconElement != null)
                {
                    if (_iconImage != null)
                        _iconElement.style.backgroundImage = new StyleBackground(_iconImage);
                    else
                        _iconElement.style.backgroundImage = StyleKeyword.None;
                }
            }
        }

        [UxmlAttribute]
        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = Mathf.Clamp01(value);
                style.opacity = _opacity;
            }
        }

        [UxmlAttribute]
        public FlexDirection direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _root.style.flexDirection = _direction;
            }
        }
        #endregion

        #region METHODS
        public LBSPanelTextIcon()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSPanelTextIcon");
            visualTree.CloneTree(this);

            _root = this.Q<VisualElement>("Root");
            
            _label = this.Q<Label>();
            _iconElement = this.Q<VisualElement>("Icon");

            // Apply initial values if set through UI Builder
            if (!string.IsNullOrEmpty(_text))
                _label.text = _text;

            if (_iconImage != null)
                _iconElement.style.backgroundImage = new StyleBackground(_iconImage);

            _iconElement.style.opacity = _opacity;

            // Set initial flex direction
            this.style.flexDirection = _direction;
        }
        #endregion
    }
}
