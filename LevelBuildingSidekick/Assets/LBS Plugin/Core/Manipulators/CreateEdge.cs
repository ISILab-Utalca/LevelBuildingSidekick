using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Obsolete] // al final no se uso, puede que se vuelva a usar
public class CreateEdge : MouseManipulator
{
    private LBSGraphController controller;
    private GraphView root;

    private static GraphElement proxyEdge;
    private static LBSNodeView first;

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

    private int n = 0;


    public void OnMouseMove(MouseMoveEvent e)
    {
        if (first == null)
            return;

        var firstPos = first.GetPosition().center;
        var mousePos = (e.localMousePosition - new Vector2(root.transform.position.x, root.transform.position.y)) / root.transform.scale;

        proxyEdge.transform.position = (firstPos + mousePos) / 2f;
        proxyEdge.transform.scale = new Vector3(.1f,Vector2.Distance(firstPos,mousePos)* 0.1f);
        n++;
        proxyEdge.transform.rotation = new Quaternion(0, 0, n, 90);
        

    }

    public void OnMouseDown(MouseDownEvent e)
    {
        if (e.button != 1)
            return;

        var target = e.currentTarget as LBSNodeView;
        if (target == null)
            return;

        first = target;
        //proxyEdge = new LBSEdgeView();
        root.AddElement(proxyEdge);
        proxyEdge.SetPosition(target.GetPosition());
    }

    public void OnMouseUp(MouseUpEvent e)
    {
        if (e.button != 1)
            return;

        if (first == null)
            return;

        var target = e.currentTarget as LBSNodeView;
        if (target != null)
        {
            var edge = new LBSEdgeData(first.Data, target.Data);
           // var view = new LBSEdgeView();
            //root.AddElement(view);
            //view.style.backgroundColor = Color.red;
            controller.AddEdge(edge);
        }
        
        var s = proxyEdge;
        root.DeleteElements(new List<GraphElement> { s });
        first = null;
        proxyEdge = null;
    }

}