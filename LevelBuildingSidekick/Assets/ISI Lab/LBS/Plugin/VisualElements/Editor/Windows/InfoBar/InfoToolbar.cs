using ISILab.Commons.Utility.Editor;
using ISILab.LBS.CustomComponents;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class InfoToolbar: VisualElement
    {
        public VisualTreeAsset VisualTree;
        
        private LBSToolbarButton clearNotificationButton;
        private LBSToolbarButton disableNotificationButton;
        private VisualElement toolInformation;

        private Label selectedLabel;
        private Label spacer;
        private Label gridText;
        private Label positionLabel;
        
        private VisualElement warningNotification;
        
        public InfoToolbar()
        {
            VisualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("InfoToolbar");
            VisualTree.CloneTree(this);

            clearNotificationButton = this.Q<LBSToolbarButton>("lbs_inspectorhide");
            
            
        }
    }
}


