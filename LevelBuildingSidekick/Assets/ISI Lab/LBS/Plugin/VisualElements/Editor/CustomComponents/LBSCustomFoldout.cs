using ISILab.Macros;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomFoldout : Foldout
    {
        private const string LBS_CLASS = "lbs-custom-foldout";
        private const string FOLDOUT_CONTENT_PANEL = "lbs-generic-panel";
        
        
        
        [UxmlAttribute]
        public string LeftIcon;
        
        
        private ToolbarMenu m_RightDropDown;
        private VisualElement m_LeftIcon;

        private VectorImage arrowDownIcon;
        private VectorImage arrowSideIcon;
        
        public LBSCustomFoldout() : base()
        {
            this.AddToClassList(LBS_CLASS);
            
            this.text = "LBS Custom Foldout";
            
            m_RightDropDown = new ToolbarMenu();
            m_LeftIcon = new VisualElement();
            
            m_LeftIcon.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            
            Toggle mToggle = this.Q<Toggle>();
            VisualElement content = this.Q<VisualElement>("unity-content");
            VisualElement arrowVisualElement = this.Q<VisualElement>("unity-checkmark");

            arrowDownIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>("b570a25de51f01c41bd82dbe5372bb3f");
            arrowSideIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>("83eafacbab9ab554299bc4d0f124d980");
            
            if (arrowDownIcon != null)
            {
                arrowVisualElement.style.backgroundImage = new StyleBackground(arrowDownIcon);
            }
            else
            {
                arrowVisualElement.style.backgroundImage = LBSAssetMacro.LoadPlaceholderTexture();
            }
            
            content.AddToClassList(FOLDOUT_CONTENT_PANEL);
            content.style.marginLeft = 0;
            
            mToggle.Add(m_RightDropDown);
            
            
        }
        
    }
}

