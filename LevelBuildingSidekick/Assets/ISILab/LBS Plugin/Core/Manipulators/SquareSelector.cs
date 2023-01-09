using LBS.Graph;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SquareSelector : MouseManipulator
{
    private Vector2Int first;
    private LBSGraphRCController controller;
    private GenericLBSWindow window;

    public SquareSelector(GenericLBSWindow window, LBSGraphRCController controller, float size = 100) // (???) deberia pedir la view en ves de la window
    {
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
        var pos = controller.ViewportMousePosition(e.localMousePosition);
        Vector2Int tpos = ToTileCoords(pos, 100);
    }

    private void OnMouseMove(MouseMoveEvent e)
    {

    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var node = e.target as LBSNodeView;
        if (node == null)
            return;

        //var edge = new LBSEdgeData(first, node.Data);
        //controller.AddEdge(edge);
        //window.RefreshView();
    }

    public Vector2Int ToTileCoords(Vector2 pos, float size) // (!) esto esta duplicado en otro manipulator, podria heredarse
    {
        int x = (pos.x > 0) ? (int)(pos.x / size) : (int)(pos.x / size) - 1;
        int y = (pos.y > 0) ? (int)(pos.y / size) : (int)(pos.y / size) - 1;

        return new Vector2Int(x, y);
    }
}
