using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DropdownToolButton : ToolButton, IGrupable
{

    private Button button;
    private VisualElement icon;
    private Label label;
    private DropdownField dropdown;

    // Event
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

    public override void AddGroupEvent(Action action)
    {
        button.clicked += action;
    }

    public override void OnBlur()
    {
        button.style.backgroundColor = color;
        base.OnBlur();
    }

    public override void OnFocus()
    {
        button.style.backgroundColor = selected;
        base.OnFocus();
    }


}
