using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.CustomComponents;
using LBS.Bundles;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
{
    
    [UxmlElement]
    public partial class BundleManagerListGroup: VisualElement
    {

        #region INTERNAL FIELDS
        private VisualElement titleCard;
        private Button leftSideButton;
        private Button rightSideButton;
        private Label titleLabel;
        private LBSCustomListView listView;
        #endregion
        
        
        #region ATRIBUTES
        [UxmlAttribute]
        public string TitleText
        {
            get => titleLabel.text;
            set
            {
                if (titleLabel != null) titleLabel.text = value;
            }
        }

        #endregion
        
        
        public BundleManagerListGroup() : base()
        {
            VisualTreeAsset vta = DirectoryTools.GetAssetByName<VisualTreeAsset>(nameof(BundleManagerListGroup));
            vta?.CloneTree(this);
            
            titleCard = this.Q<VisualElement>("TitleCard");
            leftSideButton = this.Q<Button>("ExpandButton");
            rightSideButton = this.Q<Button>("NewBundleButton");
            titleLabel = this.Q<Label>("TitleLabel");
            listView = this.Q<LBSCustomListView>("List");
            
        }

        public BundleManagerListGroup(ListView listView) : this()
        {
            //TODO: Implement this constuctor            
        }
        
    }
    
}

