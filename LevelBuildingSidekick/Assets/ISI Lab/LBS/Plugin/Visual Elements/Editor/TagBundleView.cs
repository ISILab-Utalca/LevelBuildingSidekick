using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TagBundleView : VisualElement
{
    public LBSIdentifierBundle target;

    private ListView list;
    private CustomFoldout foldout;

    public TagBundleView(LBSIdentifierBundle tagBundle)
    {
        target = tagBundle;

        if (target == null)
            return;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagBundleView");
        visualTree.CloneTree(this);

        foldout = this.Q<CustomFoldout>();

        list = this.Q<ListView>();

        Func<VisualElement> makeItem = () =>
        {
            return new TagView();
        };

        list.bindItem += (item, index) =>
        {
            var view = (item as TagView);
            var tag = this.target.GetTag(index);
            view.SetInfo(tag);
        };

        list.fixedItemHeight = 20;
        list.itemsSource = this.target.Tags;
        list.makeItem += makeItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;


    }

    private void RemoveSelected()
    {
        if (target.Tags.Count <= 0)
            return;

        var index = list.selectedIndex;
        if (index <= 0)
            return;

        var tag = target.GetTag(index);
        target.RemoveAt(index);
        list.Rebuild();
    }

    public void OnSelectionChange(IEnumerable<object> objs)
    {
        var selected = objs.ToList()[0] as LBSIdentifier;
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
