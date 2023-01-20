using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField]
    public bool UseUnitySelector;

    [NonSerialized]
    private LBSManipulator _manipulator;

    public LBSTool(Texture2D icon, string name, Type manipulator, Type inspector, bool useUnitySelector = false)
    {
        this.icon = icon;
        this.name = name;
        this.manipulator = manipulator.FullName;
        this.inspector = inspector?.FullName;
        this.UseUnitySelector = useUnitySelector;
    }


    public VisualElement GetButton(MainView view)
    {
        var btn = new BasicToolButton(this.icon, this.name);
        var mType = Type.GetType(manipulator);
        _manipulator = Activator.CreateInstance(mType) as LBSManipulator;

        btn.OnFocus += () => { 
            view.AddManipulator(_manipulator);
            if (UseUnitySelector)
            {
                view.AddManipulator(new ClickSelector());
            }
        };
        btn.OnBlur += () => { 
            view.RemoveManipulator(_manipulator); 
        };
        return btn;
    }

    public void InitManipulator(ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module) // (!)
    {
        _manipulator.InitData(ref level, ref layer, ref module);
    }

    public VisualElement GetInspector()
    {
        return null; // (!) implementar
    }
}
