using ISILab.LBS;
using ISILab.LBS.Manipulators;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GenericLBSManupulator : LBSManipulator
{
    public Action<VisualElement, Vector2Int, MouseDownEvent> _onMouseDown;
    public Action<VisualElement, Vector2Int, MouseUpEvent> _onMouseUp;

    public override void Init(LBSLayer layer, object provider)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        _onMouseDown?.Invoke(target, position, e);
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        _onMouseUp?.Invoke(target, position, e);
    }
}
