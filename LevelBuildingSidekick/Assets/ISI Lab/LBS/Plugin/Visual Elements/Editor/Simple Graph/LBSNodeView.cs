using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSNodeView<T> : GraphElement where T : LBSNode
{
    private T data;

    public T Data => data;

    public LBSNodeView(T data)
    {
        this.data = data;

        capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
        usageHints = UsageHints.DynamicTransform;

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {

    }

    private void OnMouseUp(MouseUpEvent evt)
    {

    }

    public override void OnSelected()
    {
        base.OnSelected();
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
    }

}
