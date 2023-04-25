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
    private ListView childList;
    private VisualElement contentChild;

    private Bundle target;

    private Bundle selected;

    public BundleAssetView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BundleAssetView");
        visualTree.CloneTree(this);

        this.label = this.Q<Label>("Name");
        this.icon = this.Q<VisualElement>("Icon");
        this.tab = this.Q<VisualElement>("Tab");
        this.contentChild = this.Q<VisualElement>("ContentChild");

        this.childList = this.Q<ListView>("ChildList");
        childList.makeItem = MakeItem;
        childList.bindItem = BindItem;
        childList.onItemsChosen += OnItemChosen;
        childList.onSelectionChange += OnSelectionChange;
        childList.style.flexGrow = 1.0f;
    }

    private VisualElement MakeItem()
    {
        return new BundleAssetView();
    }

    private void BindItem(VisualElement ve, int index)
    {
        var b = (CompositeBundle)target;

        if (index >= b.Bundles.Count)
            return;

        var view = (ve as BundleAssetView);
        view.SetInfo(b.Bundles[index]);
    }

    private void OnItemChosen(IEnumerable<object> objects)
    {

    }

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = objects.ToList()[0] as Bundle;

        // this.generalPanel.style.display = DisplayStyle.Flex;
        // this.characteristicsPanel.style.display = DisplayStyle.Flex;
        // this.generalPanel.SetInfo(selected);
        // this.characteristicsPanel.SetInfo(selected);
    }

    public void SetInfo(Bundle target)
    {
        this.target = target;

        this.label.text = target.name;
        this.icon.style.backgroundImage = target.ID.Icon;

        if(target.GetType() == typeof(CompositeBundle))
        {
            childList.itemsSource = ((CompositeBundle)target).Bundles;
        }
        else
        {
            contentChild.style.display = DisplayStyle.None;
        }
    }
}
