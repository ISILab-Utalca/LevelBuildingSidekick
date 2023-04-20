using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TagBundleView : VisualElement
{
    public LBSIdentifierBundle target;

    public VisualElement box;
    private Toggle toggle;
    private TextField groupNameField;
    private ListView list;

    public TagBundleView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagBundleView");
        visualTree.CloneTree(this);

        this.box = this.Q<VisualElement>("Box");
        this.toggle = this.Q<Toggle>();
        this.groupNameField = this.Q<TextField>();
        groupNameField.RegisterCallback<ChangeEvent<string>>(e => OnTextChange(e.newValue));


        list = this.Q<ListView>();

        Func<VisualElement> makeItem = () =>
        {
            return new TagView();
        };

        list.fixedItemHeight = 22;
        list.makeItem += makeItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;
        list.style.flexGrow = 1.0f;
    }

    private void OnTextChange(string value)
    {
        target.name = value;
    }


    public void SetInfo(LBSIdentifierBundle tagBundle)
    {
        target = tagBundle;

        groupNameField.value = target.name;

        list.bindItem += (item, index) =>
        {
            var view = (item as TagView);
        
            if (index >= this.target.Tags.Count())
                return;

            var tag = this.target.GetTag(index);
            view.SetInfo(tag);
        };

        list.itemsSource = this.target.Tags;

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
