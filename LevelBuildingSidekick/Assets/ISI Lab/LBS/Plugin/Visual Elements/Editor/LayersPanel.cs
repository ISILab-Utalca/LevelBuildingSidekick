using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public List<string> names = new List<string>() { "pedro", "jose", "juan" };

    public LayersPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        // LayerList
        var list = this.Q<ListView>("LayerList");

        Func<VisualElement> makeItem = () =>
        {
            return new LayerView();
        };

        list.bindItem += (item, index) =>
        {
            (item as LayerView).SetName(names[index]);
        };

        list.fixedItemHeight = 18;

        list.itemsSource = names;

        list.makeItem += makeItem;

        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange; 
    }

    public void OnSelectionChange(IEnumerable<object> objs)
    {
        Debug.Log("OSC");
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
