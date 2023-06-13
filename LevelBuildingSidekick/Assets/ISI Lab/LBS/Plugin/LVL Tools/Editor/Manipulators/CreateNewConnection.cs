using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewConnection<T> : ManipulateGraph<T> where T : LBSNode
{
    private LBSNode first;

    public CreateNewConnection()
    {
        feedback = new ConectedLine();
    }

    protected override void RegisterCallbacksOnTarget()
    {
        base.RegisterCallbacksOnTarget();
        target.RegisterCallback<MouseOverEvent>(OnMouseEnter);
        target.RegisterCallback<MouseOutEvent>(OnMouseExit);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        base.UnregisterCallbacksFromTarget();
        target.UnregisterCallback<MouseOverEvent>(OnMouseEnter);
        target.UnregisterCallback<MouseOutEvent>(OnMouseExit);
    }

    private void OnMouseEnter(MouseOverEvent evt)
    {
        var node = evt.target as LBSNodeView<T>;
        if (node == null)
            return;
        if ((node.capabilities & Capabilities.Movable) != 0)
            node.capabilities -= 8;
    }

    private void OnMouseExit(MouseOutEvent evt)
    {
        var node = evt.target as LBSNodeView<T>;
        if (node == null)
            return;

        if ((node.capabilities & Capabilities.Movable) == 0)
            node.capabilities += 8;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        var node = module.GetNode(startPosition);
        if (node == null)
            return;

        first = node;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        if (first == null)
            return;

        var node = module.GetNode(endPosition);
        if (node == null)
            return;

        var edge = new LBSEdge(first, node);
        module.AddEdge(edge);
    }
}
