using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveGraphNode<T> : ManipulateGraph<T> where T : LBSNode
{

    private LBSNode selected;

    public RemoveGraphNode() : base() { }


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
        var node = evt.target as LBSNodeView_Old<T>;
        if (node == null)
            return;
        if ((node.capabilities & Capabilities.Movable) != 0)
            node.capabilities -= 8;
    }

    private void OnMouseExit(MouseOutEvent evt)
    {
        var node = evt.target as LBSNodeView_Old<T>;
        if (node == null)
            return;

        if ((node.capabilities & Capabilities.Movable) != 0)
            node.capabilities += 8;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        var node = module.GetNode(startPosition);

        if (node == null)
        {
            return;
        }

        selected = node;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var node = module.GetNode(endPosition);

        if (node == null || !node.Equals(selected))
        {
            selected = null;
            return;
        }

        module.RemoveNode(node);

    }
}
