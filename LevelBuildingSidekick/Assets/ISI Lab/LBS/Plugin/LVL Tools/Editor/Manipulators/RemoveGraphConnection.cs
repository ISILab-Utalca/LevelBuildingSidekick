using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveGraphConnection : ManipulateGraph<RoomNode>
{
    float dist = 10;



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
        var node = evt.target as GraphElement;
        if (node == null)
            return;
        if ((node.capabilities & Capabilities.Movable) != 0)
            node.capabilities -= 8;
    }

    private void OnMouseExit(MouseOutEvent evt)
    {

        var node = evt.target as GraphElement;
        if (node == null)
            return;
        if ((node.capabilities & Capabilities.Movable) == 0)
            node.capabilities += 8;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var edge = module.GetEdge(endPosition, dist);

        if (edge == null)
            return;

        module.RemoveEdge(edge);
    }
}
