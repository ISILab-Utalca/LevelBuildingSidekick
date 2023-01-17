using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IGrupable
{
    public void AddEvent(Action action);
    public void OnBlur();
    public void OnFocus();
}