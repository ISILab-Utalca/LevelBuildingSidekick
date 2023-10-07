using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LayersPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region FIELDS
    public LBSLevelData data;

    // templates
    private List<LayerTemplate> templates;
    #endregion

    #region FIELD VIEW
    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;
    #endregion

    #region EVENTS
    public event Action<LBSLayer> OnAddLayer;
    public event Action<LBSLayer> OnRemoveLayer;
    public event Action<LBSLayer> OnSelectLayer; // click simple (!)
    public event Action<LBSLayer> OnDoubleSelectLayer; // doble click (!)
    public event Action<LBSLayer> OnLayerVisibilityChange;
    #endregion

    #region CONSTRUCTORS
    public LayersPanel() { }

    public LayersPanel(LBSLevelData data, ref List<LayerTemplate> templates)
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
            if (index >= this.data.LayerCount)
                return;

            var view = (item as LayerView);
            var layer = this.data.GetLayer(index);
            view.SetInfo(layer);
            view.OnVisibilityChange += () => { OnLayerVisibilityChange(layer); };
        };

        list.fixedItemHeight = 20;
        list.itemsSource = data.Layers;
        list.makeItem += makeItem;
        list.itemsChosen += OnItemChosen;
        list.selectionChanged += OnSelectionChange;

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
    #endregion

    #region METHODS
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
        list.selectedIndex = 0;
        OnAddLayer?.Invoke(layer);
        list.Rebuild();
    }

    private void RemoveSelectedLayer()
    {
        if (data.Layers.Count <= 0)
            return;

        var index = list.selectedIndex;
        if (index < 0)
            return;

        var answer = EditorUtility.DisplayDialog("Caution",
        "You are about to delete a layer. If you proceed with this action, all of its" +
        " content will be permanently removed, and you won't be able to recover it. Are" +
        " you sure you want to continue?", "Continue", "Cancel");

        if (!answer)
            return;

        var layer = data.RemoveAt(index);
        OnRemoveLayer?.Invoke(layer);
        list.Rebuild();

        DrawManager.ReDraw();
    }

    // Simple Click over element
    private void OnSelectionChange(IEnumerable<object> objs) 
    {
        if (objs.Count() <= 0)
            return;

        var selected = objs.ToList()[0] as LBSLayer;
        OnSelectLayer?.Invoke(selected);
    }

    // Double Click over element
    private void OnItemChosen(IEnumerable<object> objs) 
    {
        if (objs.Count() <= 0)
            return;

        var selected = objs.ToList()[0] as LBSLayer;
        OnDoubleSelectLayer?.Invoke(selected);
    }
    #endregion
}
