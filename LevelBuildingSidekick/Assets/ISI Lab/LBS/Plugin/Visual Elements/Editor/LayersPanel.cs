using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public LBSLevelData data;

    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;

    public LayersPanel(LBSLevelData data)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        this.data = data;

        // LayerList
        list = this.Q<ListView>("LayerList");

        Func<VisualElement> makeItem = () =>
        {
            return new LayerView();
        };

        list.bindItem += (item, index) =>
        {
            var view = (item as LayerView);
            var layer = data.Get(index);
            view.SetName(layer.Name);
            view.SetIcon(layer.iconPath);
        };

        list.fixedItemHeight = 18;
        list.itemsSource = data.Layers;
        list.makeItem += makeItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;

        // NameField
        nameField = this.Q<TextField>("NameField");

        // TypeDropdown
        typeDropdown = this.Q<DropdownField>("TypeDropdown");
        typeDropdown.choices = new List<string> { "Interior", "Exterior", "Population" };

        // AddLayerButton
        var addLayerBtn = this.Q<Button>("AddLayerButton");
        addLayerBtn.clicked += OnAddLayer;

        // RemoveSelectedButton
        var RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
        RemoveSelectedBtn.clicked += OnRemove;
    }

    private LBSLayer CreateLayer(int index)
    {
        LBSLayer toR = null;
        switch (index)
        {
            case 0:
                toR = new InteriorConstructor().Construct();
                break;
            case 1:
                toR = new ExteriorConstructor().Construct();
                break;
            case 2:
                toR = new PopulationConstructor().Construct();
                break;
        }
        return toR;
    }

    private void OnAddLayer()
    {
        var layer = CreateLayer(typeDropdown.index);
        layer.Name = nameField.text;
        data.AddLayer(layer);
        list.Rebuild();
    }

    private void OnRemove()
    {
        if (data.Layers.Count <= 0)
            return;

        var index = list.selectedIndex;
        if (index <= 0)
            return;

        data.RemoveAt(index);
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
