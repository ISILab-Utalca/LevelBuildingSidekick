using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionManipulator : MouseManipulator
{
    private LBSNodeData first;
    private LBSGraphRCController controller;

    public Vector2 panSpeed { get; set; }

    public ConnectionManipulator(LBSGraphRCController controller)
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
        this.controller = controller;
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
        var node = e.target as LBSNodeView;
        if (node == null)
            return;

        Debug.Log("Start drag");
        first = node.Data;//controller.GetNodeViewBylabel(node.Data.Label);
    }
    private void OnMouseMove(MouseMoveEvent e)
    {
        Debug.Log("Move drag");
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var node = e.target as LBSNodeView;
        if (node == null)
            return;

        var edge = new LBSEdgeData(first, node.Data);
        controller.AddEdge(edge);

        Debug.Log("End drag");
    }
}
