using LBS.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourBaseView : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<BehaviourBaseView, VisualElement.UxmlTraits> { }
    #endregion

    private LBSBehaviour target;

    private Button helpBtn;
    private Button settingBtn;
    private Button extraOptions;

    private VisualElement content;

    public BehaviourBaseView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BehaviourBaseView");
        visualTree.CloneTree(this);

        this.helpBtn = this.Q<Button>("HelpBtn");
        this.settingBtn = this.Q<Button>("SettingBtn");
        this.extraOptions = this.Q<Button>("ExtraOptions");

        this.content = this.Q<VisualElement>("Content");
    }

    public void SetInfo(LBSBehaviour target)
    {
        this.target = target;
    }
}
