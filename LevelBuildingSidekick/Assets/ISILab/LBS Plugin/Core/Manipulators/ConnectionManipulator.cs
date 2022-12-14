using LBS.Graph;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionManipulator : MouseManipulator
{
    private LBSNodeData first;
    private LBSGraphRCController controller;
    private GenericLBSWindow window;

    public Vector2 panSpeed { get; set; }

    public ConnectionManipulator(GenericLBSWindow window,LBSGraphRCController controller)
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
        this.controller = controller;
        this.window = window;
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

        first = node.Data;
    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        //Debug.Log("Move drag");
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
        window.RefreshView();
    }
}
