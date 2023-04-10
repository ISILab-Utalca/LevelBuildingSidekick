using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LayersPanel, VisualElement.UxmlTraits> { }

    public LBSLevelData data;

    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;

    // Events
    public event Action<LBSLayer> OnAddLayer;
    public event Action<LBSLayer> OnRemoveLayer;
    public event Action<LBSLayer> OnSelectLayer;

    // templates
    private List<LayerTemplate> templates;

    private Action onLayerVisibilityChange;

    public event Action OnLayerVisibilityChange
    {
        add => onLayerVisibilityChange += value;
        remove => onLayerVisibilityChange += value;
    }

    public LayersPanel() { }

    public LayersPanel(ref LBSLevelData data, ref List<LayerTemplate> templates)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        this.data = data;
        this.templates = templates;

        // LayerList
        list = this.Q<ListView>("LayerList");

        Func<VisualElement> makeItem = () =>
        {
            return new LayerView();
        };

        list.bindItem += (item, index) =>
        {
            var view = (item as LayerView);
            var layer = this.data.GetLayer(index);
            view.SetInfo(layer);
            view.OnVisibilityChange += onLayerVisibilityChange;
        };

        list.fixedItemHeight = 20;
        list.itemsSource = data.Layers;
        list.makeItem += makeItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;

        // NameField
        nameField = this.Q<TextField>("NameField");

        // TypeDropdown
        typeDropdown = this.Q<DropdownField>("TypeDropdown");
        typeDropdown.choices = templates.Select(t => t.name).ToList();
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
        var layers = templates.Select(t => t.layer).ToList();
        return layers[index].Clone() as LBSLayer;
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
        OnAddLayer?.Invoke(layer);
        list.Rebuild();
    }

    private void RemoveSelectedLayer()
    {
        if (data.Layers.Count <= 0)
            return;

        var index = list.selectedIndex;
        if (index <= 0)
            return;

        var layer = data.RemoveAt(index);
        OnRemoveLayer?.Invoke(layer);
        list.Rebuild();
    }

    public void OnSelectionChange(IEnumerable<object> objs)
    {
        var selected = objs.ToList()[0] as LBSLayer;
        OnSelectLayer?.Invoke(selected);
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
