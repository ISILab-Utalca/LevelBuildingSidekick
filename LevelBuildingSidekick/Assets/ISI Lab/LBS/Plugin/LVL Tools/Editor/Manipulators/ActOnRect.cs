using ISILab.AI.Optimization.Populations;
using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ActOnRect : LBSManipulator
{
    Action<Rect> OnSelection;

    private LBSLayer layer;

    public ActOnRect(Action<Rect> action) 
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
        OnSelection = action;
    }

    public override void Init(LBSLayer layer, object provider)
    {
        this.layer = layer;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var corners = layer.ToFixedPosition(StartPosition, EndPosition);

        var size = corners.Item2 - corners.Item1 + Vector2.one; 
        var r = new Rect(corners.Item1, size);
        Debug.Log(r);
        OnSelection?.Invoke(r);
    }
}
