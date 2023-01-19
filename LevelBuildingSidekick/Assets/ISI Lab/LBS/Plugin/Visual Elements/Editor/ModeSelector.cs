using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class ModeSelector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModeSelector, VisualElement.UxmlTraits> { }

    private Button button;
    private DropdownField dropdown;

    private Dictionary<string, object> objects;

    public event Action<string> OnSelectionChange;

    public int Index
    {
        get => dropdown.index;
        set => dropdown.index = value;
    }

    public ModeSelector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ModeSelector"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>();

        // Dropdown
        dropdown = this.Q<DropdownField>();
        dropdown.RegisterCallback<ChangeEvent<string>>(e => OnSelectionChange?.Invoke(e.newValue));
    }

    public void SetChoices(Dictionary<string,object> objects)
    {
        this.objects = objects;
        dropdown.choices = objects.Select(o => o.Key).ToList();
    }

    public object GetSelection(string name)
    {
        object obj;
        objects.TryGetValue(name, out obj);
        return obj;
    }

}
