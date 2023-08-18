using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewRoomNode : ManipulateGraph<RoomNode> 
{
    private string prefix = "";
    private string postfix = "";

    public CreateNewRoomNode() : base() 
    {
        this.prefix = "Node: ";
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var pos = MainView.FixPos(e.localMousePosition);
        //var pos = e.localMousePosition;

        var name = "";
        var loop = true;
        var v = 0;
        do
        {
            name = prefix + v + postfix;

            var nn = module.GetNode(name);
            if (nn == null)
                loop = false;
            v++;
        } while (loop);

        var n = new RoomNode(name, pos.ToInt(), new RoomData()); //var n = Activator.CreateInstance<T>();
        n.Height = n.Width = 3;
        module.AddNode(n);
    }

}
