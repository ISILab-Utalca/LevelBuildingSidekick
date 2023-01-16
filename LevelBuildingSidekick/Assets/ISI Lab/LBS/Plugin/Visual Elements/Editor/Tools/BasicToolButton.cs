using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicToolButton : VisualElement, IGrupable
{
    public Color color = new Color(72, 72, 72);
    public Color selected = new Color(207, 100, 29);

    private Button button;
    private VisualElement icon;
    private Label label;

    public BasicToolButton()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BasicToolButton"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>("Button");

        // Icon
        icon = this.Q<VisualElement>("Icon");

        // LabelName
        label = this.Q<Label>("LabelName");

    }

    public void SetEvent(Action action)
    {
        button.clicked += action;
    }

    public void SetActive(bool value)
    {
        button.style.backgroundColor = (value) ? selected: color;
    }
}

