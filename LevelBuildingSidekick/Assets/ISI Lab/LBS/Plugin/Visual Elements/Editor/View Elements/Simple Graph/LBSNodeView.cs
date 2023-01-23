using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSNodeView<T> : GraphElement where T : LBSNode
{
    private T data;

    public T Data => data;

    public Action<Vector2Int> OnMoving;

    // Visual elements
    public Label label;

    public LBSNodeView(T data, Vector2 position, Vector2 size)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("NodeUxml");
        visualTree.CloneTree(this);

        this.data = data;

        capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
        usageHints = UsageHints.DynamicTransform;

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);

        this.SetPosition(new Rect(position, size));

        // Label
        label = this.Q<Label>();
    }

    private void OnMouseDown(MouseDownEvent evt)
    {

    }

    private void OnMouseUp(MouseUpEvent evt)
    {

    }

    /// <summary>
    /// Set a new position by parameter.
    /// </summary>
    /// <param name="newPos"> New position given.</param>
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        var center = base.GetPosition().center;
        var nPos = new Vector2Int((int)center.x, (int)center.y);
        Data.Position = nPos;
        OnMoving?.Invoke(nPos);
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