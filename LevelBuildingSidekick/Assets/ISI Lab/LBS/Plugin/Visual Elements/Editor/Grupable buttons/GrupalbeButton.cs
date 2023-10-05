using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrupalbeButton : Button, IGrupable
{
    public new class UxmlFactory : UxmlFactory<GrupalbeButton, Button.UxmlTraits> { }

    public string label;

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

    public GrupalbeButton() { }

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

    public string GetLabel()
    {
        return this.label;
    }
}

