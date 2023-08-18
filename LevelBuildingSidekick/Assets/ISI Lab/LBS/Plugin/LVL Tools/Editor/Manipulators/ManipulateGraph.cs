using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManipulateGraph<T> : LBSManipulator where T : LBSNode
{
    protected GraphModule<T> module;

    public override void Init(LBSLayer layer, LBSBehaviour behaviour)
    {
        this.module = layer.GetModule<GraphModule<T>>();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        throw new System.NotImplementedException();
    }
}
