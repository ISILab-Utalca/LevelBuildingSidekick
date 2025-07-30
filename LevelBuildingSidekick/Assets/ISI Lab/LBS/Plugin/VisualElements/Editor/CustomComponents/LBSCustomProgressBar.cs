using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomProgressBar: ProgressBar
    {
        
        #region Constants
        public static readonly string LBSClassName = "lbs-field";
        #endregion

        #region Fields

        private const string STYLE_CLASS_NAME = "lbs-custom-progress-bar";
        
        private Color progressThemeColor = Color.blue;

        
        #endregion
        
        #region VisualElements
        private VisualElement barVisualElement;
        private VectorImage iconImage;
        
        private VisualElement icon;
        private Label progressLabel;

        #endregion
        
        #region Properties
        [UxmlAttribute]
        public Color ProgressThemeColor
        {
            get => progressThemeColor;
            set
            {
                progressThemeColor = value;
                UpdateBarColorTheme(value);
            }
        }

        [UxmlAttribute]
        public VectorImage ProgressIconImage
        {
            get => iconImage;
            set
            {
                iconImage = value;
                if (iconImage != null)
                {
                    icon.style.backgroundImage = new StyleBackground(iconImage);
                }
            }
        }

        #endregion

        public LBSCustomProgressBar() : base()
        {
            this.RemoveFromClassList(ussClassName);
            this.AddToClassList(STYLE_CLASS_NAME);
            
            VisualElement topPanel = new VisualElement();
            topPanel.AddToClassList("lbs-progress-bar-top-panel");
            topPanel.style.flexDirection = FlexDirection.Row;
            this.Add(topPanel);

            VisualElement container = this.Query<VisualElement>(className: containerUssClassName);
            container.AddToClassList(STYLE_CLASS_NAME + "__container");
            topPanel.PlaceBehind(container);

            VisualElement progressbarBackground = this.Query<VisualElement>(classes: backgroundUssClassName);
            progressbarBackground.AddToClassList(STYLE_CLASS_NAME + "__background");
            
            icon = new VisualElement();
            icon.AddToClassList(STYLE_CLASS_NAME + "__icon");
            icon.style.backgroundImage = new StyleBackground(Macros.LBSAssetMacro.LoadPlaceholderTexture());
            icon.style.width = 16;
            icon.style.height = 16;
            topPanel.Add(icon);
            
            progressLabel = new Label("Progress Label");
            progressLabel.AddToClassList(STYLE_CLASS_NAME + "__progress-label");
            progressLabel.AddToClassList("unity-base-field__label");
            topPanel.Add(progressLabel);
            
            barVisualElement = this.Query<VisualElement>(className: progressUssClassName);
            barVisualElement.AddToClassList(STYLE_CLASS_NAME + "__progress");
            
            
            
            UpdateBarColorTheme(ProgressThemeColor);
            
            
        }


        private void UpdateBarColorTheme(Color _color)
        {
            Color bgColor = _color;
            if (barVisualElement != null)
            {
                barVisualElement.style.backgroundColor = bgColor;
            }
            bgColor.a = 0.2f;
            this.style.backgroundColor = bgColor;
        }
    }
}


