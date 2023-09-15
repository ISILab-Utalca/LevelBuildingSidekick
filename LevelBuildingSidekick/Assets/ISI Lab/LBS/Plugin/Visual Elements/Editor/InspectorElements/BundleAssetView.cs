using LBS.Bundles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BundleAssetView : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<BundleAssetView, VisualElement.UxmlTraits> { }
    #endregion

    private Label label;
    private VisualElement icon;
    private VisualElement tab;

    private Bundle target;

    public BundleAssetView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BundleAssetView");
        visualTree.CloneTree(this);

        this.label = this.Q<Label>("Name");
        this.icon = this.Q<VisualElement>("Icon");
        this.tab = this.Q<VisualElement>("Tab");
    }

    public void SetInfo(Bundle target,int value)
    {
        this.target = target;

        this.label.text = target.name;

        this.icon.style.backgroundImage = target.Icon;

        tab.style.width =  20 * value;
    }
}
