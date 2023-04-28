using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalBehaviours : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalBehaviours, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private VisualElement noContentPanel;

    private LBSLayer target;

    public LBSLocalBehaviours()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
    }

    public void SetInfo(LBSLayer target)
    {
        //this.target = target.Assitant.
    }
}
