using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateEdge : MouseManipulator
{
    private LBSGraphController controller;
    private GraphView root;

    private GraphElement proxyEdge;
    private LBSNodeView first;

    public CreateEdge(Controller controller, GraphView root)
    {
        this.root = root;
        this.controller = controller as LBSGraphController;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    public void OnMouseMove(MouseMoveEvent e)
    {
        if (first == null)
            return;

        var target = e.currentTarget as LBSNodeView;
        if (target == null)
            return;

        proxyEdge.transform.scale += new Vector3(10,10,10);

    }

    public void OnMouseDown(MouseDownEvent e)
    {
        var target = e.currentTarget as LBSNodeView;
        if (target == null)
            return;

        first = target;
        proxyEdge = new LBSEdgeView();
        root.Add(proxyEdge);
        proxyEdge.transform.position = target.transform.position;
    }

    public void OnMouseUp(MouseUpEvent e)
    {
        var target = e.currentTarget as LBSNodeView;
        if (target != null)
        {
            var edge = new LBSEdgeData(first.Data, target.Data);
            controller.AddEdge(edge);
        }
        
        first = null;
        root.DeleteElements(new List<GraphElement> { proxyEdge });
        proxyEdge = null;
    }

}