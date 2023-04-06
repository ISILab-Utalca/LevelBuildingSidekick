using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrupalbeButton : Button, IGrupable
{
    public new class UxmlFactory : UxmlFactory<GrupalbeButton, Button.UxmlTraits> { }

    private Color color = new Color(1, 0, 0);
    private Color selected = new Color(0, 0, 1);

    private Action OnFocusEvent;
    private Action OnBlurEvent;

    event Action IGrupable.OnFocusEvent
    {
        add => OnFocusEvent += value;
        remove => OnFocusEvent -= value;
    }

    event Action IGrupable.OnBlurEvent
    {
        add => OnBlurEvent += value;
        remove => OnBlurEvent -= value;
    }

    public GrupalbeButton()
    {

    }

    public GrupalbeButton(string text )
    {
        this.text = text;
    }

    public void AddGroupEvent(Action action)
    {
        this.clicked += action;
    }

    public void OnBlur()
    {
        this.style.backgroundColor = color;
        OnBlurEvent?.Invoke();
    }

    public void OnFocus()
    {
        this.style.backgroundColor = selected;
        OnFocusEvent?.Invoke();
    }

    public void SetColorGroup(Color color, Color selected)
    {
        this.color = color;
        this.selected = selected;
    }
}

public class SimpleGrupableButton : LBSGrupableButton
{
    public new class UxmlFactory : UxmlFactory<SimpleGrupableButton, VisualElement.UxmlTraits> { }

    public Button button;

    public SimpleGrupableButton()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("SimpleGrupableButton");
        visualTree.CloneTree(this);

        button = this.Q<Button>();
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

    internal void SetName(string v)
    {
        button.text = v;
    }
}
