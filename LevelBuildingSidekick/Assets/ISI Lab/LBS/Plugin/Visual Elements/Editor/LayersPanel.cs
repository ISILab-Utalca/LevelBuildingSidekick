using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public List<string> names = new List<string>() { };

    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;

    public LayersPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        // LayerList
        list = this.Q<ListView>("LayerList");

        Func<VisualElement> makeItem = () =>
        {
            return new LayerView();
        };

        list.bindItem += (item, index) =>
        {
            var view = (item as LayerView);
            view.SetName(names[index]);
        };

        list.fixedItemHeight = 18;
        list.itemsSource = names;
        list.makeItem += makeItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;

        // NameField
        nameField = this.Q<TextField>("NameField");

        // TypeDropdown
        typeDropdown = this.Q<DropdownField>("TypeDropdown");

        // AddLayerButton
        var addLayerBtn = this.Q<Button>("AddLayerButton");
        addLayerBtn.clicked += OnAddLayer;

        // RemoveSelectedButton
        var RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
        RemoveSelectedBtn.clicked += OnRemove;
    }

    public void Init()
    {

    }

    private void OnAddLayer()
    {
        names.Add(nameField.text);
        list.Rebuild();
    }

    private void OnRemove()
    {
        if (names.Count <= 0)
            return;

        var index = list.selectedIndex;
        names.RemoveAt(index);
        list.Rebuild();
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
