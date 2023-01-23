using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewConnection<T> : LBSManipulator where T : LBSNode
{
    private GraphModule<T> module;

    private LBSNode first;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<GraphModule<T>>();
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var node = e.target as LBSNodeView<T>;
        if (node == null)
            return;

        first = node.Data;
    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        //Debug.Log("Move drag");
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var node = e.target as LBSNodeView<T>;
        if (node == null)
            return;

        var edge = new LBSEdge(first, node.Data);
        module.AddEdge(edge);
        OnManipulationEnd?.Invoke();
    }
}
