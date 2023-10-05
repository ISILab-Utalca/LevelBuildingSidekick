using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolButton : VisualElement, IGrupable
{
    #region FIELDS
    public Color color = new Color(72f / 255f, 72f / 255f, 72f / 255f);
    public Color selected = new Color(161f / 255f, 81f / 255f, 21f / 255f);
    #endregion

    #region FIELDS VIEW
    private Button button;
    private VisualElement icon;
    #endregion

    #region EVENTS
    public event Action OnFocusEvent;
    public event Action OnBlurEvent;
    #endregion

    #region CONSTRUCTORS
    public ToolButton(LBSTool tool)
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
    #endregion

    #region IGRUPABLE
    public void AddGroupEvent(Action action)
    {
        button.clicked += action;
    }

    public void OnBlur()
    {
        button.style.backgroundColor = color;
        OnBlurEvent?.Invoke();
    }

    public void OnFocus()
    {
        button.style.backgroundColor = selected;
        OnFocusEvent?.Invoke();
    }

    public void SetColorGroup(Color color, Color selected)
    {
        this.color = color;
        this.selected = selected;
    }

    public string GetLabel()
    {
        return this.tooltip;
    }
    #endregion
}