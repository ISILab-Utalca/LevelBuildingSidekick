using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public LayersPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        // LayerList
        var list = this.Q<ListView>("LayerList");
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (l, i) => (l as Label).text = i.ToString();
        //Func<VisualElement> makeItem = () => new LayerView();
        //Action<VisualElement, int> bindItem = (l, n) => (l as LayerView).SetName(n.ToString());
        list.fixedItemHeight = 16;
        list.makeItem += makeItem;
        list.bindItem += bindItem;
        //list.unbindItem += ;
        //list.itemsAdded += ;
        //list.itemsRemoved += ;
        //list.itemsSource += ;
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
