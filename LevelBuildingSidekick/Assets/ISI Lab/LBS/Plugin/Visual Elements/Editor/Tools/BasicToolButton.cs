using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicToolButton : ToolButton
{
    private Button button;
    private VisualElement icon;
    private Label label;

    public BasicToolButton(Texture2D texture, string name) : base()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BasicToolButton"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>("Button");
        //button.clicked += OnFocus;

        // Icon
        icon = this.Q<VisualElement>("Icon");
        icon.style.backgroundImage = texture;

        // LabelName
        label = this.Q<Label>("LabelName");
        label.text = name;
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

