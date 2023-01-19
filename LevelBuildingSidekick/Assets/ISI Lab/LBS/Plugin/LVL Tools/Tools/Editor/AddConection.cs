using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddConection<T> : LBSManipulator where T : LBSNode
{
    private LBSNode first;

    public override void InitData(ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        throw new System.NotImplementedException();
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

    }
}
