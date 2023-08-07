using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolButton : VisualElement, IGrupable
{
    // View
    private Button button;
    private VisualElement icon;

    public ToolButton(LBSTool tool): base()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolButton");
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>("Button");

        // Icon
        icon = this.Q<VisualElement>("Icon");
        icon.style.backgroundImage = tool.Icon;

        this.tooltip = tool.Name;
    }

    public event Action OnFocusEvent;
    public event Action OnBlurEvent;

    public void AddGroupEvent(Action action)
    {
        button.clicked += action;
    }

    public void OnBlur()
    {
        //button.style.backgroundColor = color;
        OnBlurEvent?.Invoke();
    }

    public void OnFocus()
    {
        //button.style.backgroundColor = selected;
        OnFocusEvent?.Invoke();
    }

    public void SetColorGroup(Color color, Color select)
    {
        throw new NotImplementedException();
    }
}

