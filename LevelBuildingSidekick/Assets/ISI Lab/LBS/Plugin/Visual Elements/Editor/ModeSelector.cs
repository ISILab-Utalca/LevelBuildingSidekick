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
    #region UXML_FACTORY
    
    public new class UxmlFactory : UxmlFactory<ModeSelector, VisualElement.UxmlTraits> { }

    #endregion

    #region FIELDS

    private Button button;
    private DropdownField dropdown;

    private Dictionary<string, object> objects;

    #endregion

    #region EVENTS

    public event Action<string> OnSelectionChange;
    public event Action OnUpdateMode;

    #endregion

    #region PROPERTIES

    public int Index
    {
        get => dropdown.index;
        set => dropdown.index = value;
    }

    #endregion

    #region CONSTRUCTORS

    public ModeSelector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ModeSelector"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>();
        button.clicked += () => OnUpdateMode.Invoke();

        // Dropdown
        dropdown = this.Q<DropdownField>();
    }

    #endregion

    #region METHODS

    public void SetChoices(Dictionary<string,object> objects)
    {
        this.objects = objects;
        dropdown.choices = objects.Select(o => o.Key).ToList();
    }

    public void Disable()
    {
        dropdown.UnregisterCallback<ChangeEvent<string>>(SelectionChanged);
    }

    public void Enable()
    {
        dropdown.RegisterCallback<ChangeEvent<string>>(SelectionChanged);
    }

    private void SelectionChanged(ChangeEvent<string> e)
    {
        OnSelectionChange?.Invoke(e.newValue);
    }

    public int GetChoiceIndex(string choice)
    {
        return dropdown.choices.FindIndex(c => c == choice);
    }

    public object GetSelection(string name)
    {
        object obj;
        objects.TryGetValue(name, out obj);
        return obj;
    }

    #endregion
}
