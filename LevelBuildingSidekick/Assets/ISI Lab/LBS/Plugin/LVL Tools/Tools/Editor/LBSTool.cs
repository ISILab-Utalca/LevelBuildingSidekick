using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LBSTool
{
    [SerializeField]
    public Texture2D icon;
    [SerializeField]
    public string name;
    [SerializeField]
    public string manipulator;
    [SerializeField]
    public string inspector;

    public LBSTool(Texture2D icon, string name, Type manipulator, Type inspector)
    {
        this.icon = icon;
        this.name = name;
        this.manipulator = manipulator.FullName;
        this.inspector = inspector?.FullName;
    }

    public VisualElement GetButton(MainView view)
    {
        var btn = new BasicToolButton(this.icon, this.name);
        var mType = Type.GetType(manipulator);
        var mObj = Activator.CreateInstance(mType) as Manipulator;

        btn.OnFocus += () => { view.AddManipulator(mObj); };
        btn.OnBlur += () => { view.RemoveManipulator(mObj); };
        return btn;
    }

    public VisualElement GetInspector()
    {
        return null; // (!) implementar
    }
}
