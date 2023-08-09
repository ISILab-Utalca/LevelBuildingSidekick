using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
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


    public void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
    }

    protected override void RegisterCallbacksOnTarget()
    {
        base.RegisterCallbacksOnTarget();
        target.RegisterCallback<MouseDownEvent>(_OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(_OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(_OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        base.UnregisterCallbacksFromTarget();
        target.UnregisterCallback<MouseDownEvent>(_OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(_OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(_OnMouseUp);
    }

    private void _OnMouseDown(MouseDownEvent evt)
    {
        OnManipulationStart?.Invoke();

    }

    private void _OnMouseMove(MouseMoveEvent evt)
    {

    }

    private void _OnMouseUp(MouseUpEvent evt)
    {

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
