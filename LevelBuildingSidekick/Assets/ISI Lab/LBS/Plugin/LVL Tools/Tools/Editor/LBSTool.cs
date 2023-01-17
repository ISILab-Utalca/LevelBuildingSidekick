using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class LBSTool
{
    public Texture2D icon;
    public string name;

    public Action OnFocus;
    public Action OnBlur;

    public abstract VisualElement GetButton();

    public abstract VisualElement GetInspector();
}
