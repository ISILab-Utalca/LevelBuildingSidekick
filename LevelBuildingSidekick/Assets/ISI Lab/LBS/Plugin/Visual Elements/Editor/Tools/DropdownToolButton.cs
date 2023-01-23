using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DropdownToolButton : VisualElement, IGrupable
{
    public Color color = new Color(72 / 255f, 72 / 255f, 72 / 255f);
    public Color selected = new Color(161 / 255f, 81 / 255f, 21 / 255f);

    private Button button;
    private VisualElement icon;
    private Label label;
    private DropdownField dropdown;

    // Event
    public event Action OnFocus; // se llama cuando se selecciona el boton
    public event Action OnBlur; // se llama cuando se dejo de seleccionar el boton
    public event Action<int,string> OnModeChange;

    public DropdownToolButton(Texture2D texture, string name, List<string> choices)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("DropdownToolButton"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>("Button");

        // Icon
        icon = this.Q<VisualElement>("Icon");
        icon.style.backgroundImage = texture;

        // LabelName
        label = this.Q<Label>("Label");
        label.text = name;

        // Dropdown
        dropdown = this.Q<DropdownField>("Dropdown");
        dropdown.choices = choices;
        dropdown.index = 0;
        dropdown.RegisterCallback<ChangeEvent<string>>(e => {
            OnModeChange?.Invoke(dropdown.index, e.newValue);
        });
    }

    public void AddGroupEvent(Action action)
    {
        button.clicked += action;
    }

    void IGrupable.OnBlur()
    {
        button.style.backgroundColor = color;
        OnBlur?.Invoke();
    }

    void IGrupable.OnFocus()
    {
        button.style.backgroundColor = selected;
        OnFocus?.Invoke();
    }
}
