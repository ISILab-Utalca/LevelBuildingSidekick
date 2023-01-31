using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class LBSMultiTool : LBSTool
{
    [SerializeField]
    private List<string> modes;
    [SerializeField]
    private List<string> manipulators = new List<string>();

    private List<IManipulatorLBS> _manipulators = new List<IManipulatorLBS>();

    public LBSMultiTool(Texture2D icon, string name,List<string> modes, List<Type> manipulators, Type inspector, bool useUnitySelector = false) : base(icon, name, manipulators[0], inspector, useUnitySelector)
    {
        this.modes = modes;
        foreach (var manipulator in manipulators)
        {
            this.manipulators.Add(manipulator.FullName);
        }
    }

    public override LBSGrupableButton InitButton(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        _manipulators = new List<IManipulatorLBS>();
        base.InitButton(view, ref level, ref layer, ref module); // (?) inecesario?
        foreach (var manipulator in manipulators)
        {
            var mType = Type.GetType(manipulator);
            var current = Activator.CreateInstance(mType) as IManipulatorLBS;
            current.AddManipulationStart(OnStartAction);
            current.AddManipulationUpdate(OnUpdateAction);
            current.AddManipulationEnd(OnEndAction);
            _manipulator.AddManipulationEnd(() => Debug.Log("Mani: " + _manipulator.GetType().ToString()));

            current.Init(ref view, ref level, ref layer, ref module);
            _manipulators.Add(current);

        }

        var btn = new DropdownToolButton(this.icon, this.name, modes);
        btn.OnModeChange += (index, name) => SetManipulator(index, view, btn);

        SetManipulator(2, view, btn);

        //btn.OnFocusEvent += () => {
            //view.AddManipulator(_manipulator as Manipulator);
            //if (UseUnitySelector)
            //{
            //    view.AddManipulator(new ClickSelector());
            //}
        //};
        //btn.OnBlurEvent += () => {
        //    view.RemoveManipulator(_manipulator as Manipulator);
        //};
        return btn;
    }

    public override LBSInspector InitInspector(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        var iType = Type.GetType(this.inspector);
        _inspector = Activator.CreateInstance(iType) as LBSInspector;
        _inspector.Init(new List<IManipulatorLBS>(_manipulators), ref view, ref level, ref layer, ref module);

        return _inspector;
    }

    private void SetManipulator(int n, MainView view, DropdownToolButton button)
    {
        view.RemoveManipulator(_manipulator as Manipulator);
        _manipulator = _manipulators[n];

        button.OnFocusEvent += () => {
            view.AddManipulator(_manipulator as Manipulator);
        };
        button.OnBlurEvent += () => {
            view.RemoveManipulator(_manipulator as Manipulator);
        };
    }
}
