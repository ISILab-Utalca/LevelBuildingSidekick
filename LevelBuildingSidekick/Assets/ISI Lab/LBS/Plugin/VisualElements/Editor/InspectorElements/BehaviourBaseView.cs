using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class BehaviourBaseView : VisualElement
    {
        #region FACTORY
      //  public new class UxmlFactory : UxmlFactory<BehaviourBaseView, UxmlTraits> { }
        #endregion

        private LBSBehaviour target;

        private Button helpBtn;
        private Button settingBtn;
        private Button extraOptions;

        private VisualElement content;

        public BehaviourBaseView()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BehaviourBaseView");
            visualTree.CloneTree(this);

            helpBtn = this.Q<Button>("HelpBtn");
            settingBtn = this.Q<Button>("SettingBtn");
            extraOptions = this.Q<Button>("ExtraOptions");

            content = this.Q<VisualElement>("Content");
        }

        public void SetInfo(LBSBehaviour target)
        {
            this.target = target;
        }
    }
}