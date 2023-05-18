using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewConnection<T> : LBSManipulator where T : LBSNode
{
    private GraphModule<T> module;

    private LBSNodeView<T> first;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<GraphModule<T>>();
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseOverEvent>(OnMouseEnter);
        target.RegisterCallback<MouseOutEvent>(OnMouseExit);
    }

    private void OnMouseEnter(MouseOverEvent evt)
    {
        var node = evt.target as LBSNodeView<T>;
        if (node != null)
            node.capabilities -= 8;
        Debug.Log(evt.target);
    }

    private void OnMouseExit(MouseOutEvent evt)
    {
        var node = evt.target as LBSNodeView<T>;
        if (node == null)
            return;

        node.capabilities += 8;
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseOverEvent>(OnMouseEnter);
        target.UnregisterCallback<MouseOutEvent>(OnMouseExit);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var node = e.target as LBSNodeView<T>;
        if (node == null)
            return;

        //node.capabilities -= 8;

        first = node;
    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        //Debug.Log("Move drag");
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;
        //first.capabilities += 8;

        var node = e.target as LBSNodeView<T>;
        if (node == null)
            return;

        var edge = new LBSEdge(first.Data, node.Data);
        module.AddEdge(edge);
        OnManipulationEnd?.Invoke();
    }
}
