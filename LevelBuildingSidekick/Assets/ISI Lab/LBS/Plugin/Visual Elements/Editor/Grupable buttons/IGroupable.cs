using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IGrupable
{
    // Event
    public event Action OnFocusEvent;
    public event Action OnBlurEvent;

    public string GetLabel();
    public void SetColorGroup(Color color, Color select);
    public void AddGroupEvent(Action action);
    public void OnBlur();
    public void OnFocus();
}