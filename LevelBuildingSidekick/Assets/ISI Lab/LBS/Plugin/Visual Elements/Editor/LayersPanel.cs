using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public LBSLevelData data;

    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;

    public event Action<LBSLayer> OnAddLayer;
    public event Action<LBSLayer> OnRemoveLayer;
    public event Action<LBSLayer> OnSelectLayer;

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
        typeDropdown.index = 0; // (?)

        // AddLayerButton
        var addLayerBtn = this.Q<Button>("AddLayerButton");
        addLayerBtn.clicked += AddLayer;

        // RemoveSelectedButton
        var RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
        RemoveSelectedBtn.clicked += RemoveSelectedLayer;
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

    private void AddLayer()
    {
        if (typeDropdown.index < 0)
        {
            Debug.LogWarning("No layer type has been selected yet, make sure to select one.");
            return;
        }

        var layer = CreateLayer(typeDropdown.index);

        layer.Name = nameField.text;

        int i = 1;
        while (data.Layers.Any(l => l.Name.Equals(layer.Name)))
        {
            layer.Name = nameField.text + " " + i;
            i++;
        }

        data.AddLayer(layer);
        list.Rebuild();
    }

    private void RemoveSelectedLayer()
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
        OnSelectLayer(objs.ToList()[0] as LBSLayer);
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
