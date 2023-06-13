using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveGraphConnection : ManipulateGraph<RoomNode>
{
    float dist = 25;
    
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
