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

    Vector2 start;

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
        start = startPosition;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var end = endPosition;
        var x = start.x < end.x ? start.x : end.x;
        var y = start.y < end.y ? start.y : end.y;
        var with = start.x > end.x ? start.x : end.x;
        var height = start.y > end.y ? start.y : end.y;

        var pos = layer.ToFixedPosition(new Vector2(x,y));
        var size = layer.ToFixedPosition(new Vector2(with, height)) - pos + Vector2Int.one; 
        var r = new Rect(pos, size);
        OnSelection?.Invoke(r);
    }
}
