using ISILab.Macros;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomFoldout : Foldout
    {

        //Foldout
        public const string FOLDOUT_USS = "lbs-foldout";
        public const string FOLDOUT_CONTENT_PANEL = "lbs-foldout-panel";
        public const string FOLDOUT_CONTENT_MENU_BUTTON = "lbs-foldout-menu-button";
        public const string FOLDOUT_CHECKMARK = "lbs-custom-foldout-checkmark";

        
        
        private VectorImage arrowDownIcon;
        private VectorImage arrowSideIcon;
        private VectorImage dotsIcon;
        
        private ToolbarMenu m_RightDropDown;
        private VisualElement m_LeftIcon;
        private VisualElement arrowVisualElement;
        private VisualElement content;
            
        [UxmlAttribute]
        public string LeftIcon;
        
        
        public LBSCustomFoldout() : base()
        {
            this.AddToClassList(FOLDOUT_USS);
            
            this.text = "LBS Custom Foldout";
            
            m_RightDropDown = new ToolbarMenu();
            m_RightDropDown.AddToClassList(FOLDOUT_CONTENT_MENU_BUTTON);
            m_RightDropDown.RemoveFromClassList(ToolbarMenu.ussClassName);
            
            m_LeftIcon = new VisualElement();
            
            m_LeftIcon.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            
            Toggle mToggle = this.Q<Toggle>();
            mToggle.RemoveFromClassList(Toggle.ussClassName);
            mToggle.RemoveFromClassList(toggleUssClassName);
            mToggle.AddToClassList(FOLDOUT_USS + "__toggle");
            
            content = this.Q<VisualElement>("unity-content");
            arrowVisualElement = this.Q<VisualElement>("unity-checkmark");
            arrowVisualElement.AddToClassList(FOLDOUT_CHECKMARK);
            
            Label contentLabel = this.Q<Label>(classes: textUssClassName);
            contentLabel.AddToClassList("unity-base-field__label");
            
            
            arrowDownIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>("b570a25de51f01c41bd82dbe5372bb3f");
            arrowSideIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>("83eafacbab9ab554299bc4d0f124d980");
            dotsIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>("4fc870f9e2f488d4bb2c1bffe1f5b751");
            

            
            if (arrowDownIcon != null)
            {
                if (value)
                {
                    arrowVisualElement.style.backgroundImage = new StyleBackground(arrowDownIcon);
                }
                else
                {
                    arrowVisualElement.style.backgroundImage = new StyleBackground(arrowSideIcon);
                }
                
            }
            else
            {
                arrowVisualElement.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            }
            
            content.AddToClassList(FOLDOUT_CONTENT_PANEL);
            content.style.marginLeft = 0;
            
            mToggle.Add(m_RightDropDown);
            
            VisualElement toolbarButtonIcon = this.Query<VisualElement>(classes: "unity-toolbar-menu__arrow");
            TextElement toolbarLabel  = m_RightDropDown.Query<TextElement>(classes: "unity-toolbar-menu__text");
            toolbarLabel.style.display = DisplayStyle.None;
            
            if (toolbarButtonIcon != null)
            {
                toolbarButtonIcon.style.backgroundImage = new StyleBackground(dotsIcon);
            }

            mToggle.RegisterValueChangedCallback(OnChangeEvent);

        }

        void OnChangeEvent(ChangeEvent<bool> _evt)
        {
            if (_evt.newValue)
            {
                arrowVisualElement.style.backgroundImage = new StyleBackground(arrowDownIcon);
            }
            else
            {
                arrowVisualElement.style.backgroundImage = new StyleBackground(arrowSideIcon);
            }
            
            _evt.StopPropagation();
        }
    }
}

