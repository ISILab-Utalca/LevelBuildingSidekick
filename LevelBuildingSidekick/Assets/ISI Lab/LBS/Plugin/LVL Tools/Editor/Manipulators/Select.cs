using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Select : ClickSelector , IManipulatorLBS
{
    public Action OnManipulationStart;
    public Action OnManipulationUpdate;
    public Action OnManipulationEnd;

    public void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        // do nothing
    }

    protected override void RegisterCallbacksOnTarget()
    {
        base.RegisterCallbacksOnTarget();
        target.RegisterCallback<MouseDownEvent>(_OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        base.UnregisterCallbacksFromTarget();
        target.UnregisterCallback<MouseDownEvent>(_OnMouseDown);
    }

    private void _OnMouseDown(MouseDownEvent evt)
    {
        OnManipulationStart?.Invoke();
        // Do nothing
        OnManipulationEnd?.Invoke();
    }

    public void AddManipulationEnd(Action action)
    {
        OnManipulationEnd += action;
    }

    public void AddManipulationStart(Action action)
    {
        OnManipulationStart += action;
    }

    public void AddManipulationUpdate(Action action)
    {
        OnManipulationUpdate += action;
    }

    public void RemoveManipulationEnd(Action action)
    {
        OnManipulationEnd -= action;
    }

    public void RemoveManipulationStart(Action action)
    {
        OnManipulationStart -= action;
    }

    public void RemoveManipulationUpdate(Action action)
    {
        OnManipulationUpdate -= action;
    }
}
