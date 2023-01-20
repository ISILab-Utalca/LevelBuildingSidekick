using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewRoomNode : LBSManipulator // where T: LBSNode  // (!) CreateNewNode<T>
{
    // ref Data
    private GraphModule<RoomNode> module;

    public override void InitData(ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<GraphModule<RoomNode>>();
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        var pos = e.localMousePosition;

        var name = "";
        var loop = true;
        var v = 0;
        do
        {
            name = "Node: " + v;

            var nn = module.GetNode(name);
            if (nn == null)
                loop = false;
            v++;
        } while (loop);

        var n = new RoomNode(name, pos.ToInt(), new RoomData()); //var n = Activator.CreateInstance<T>();
        module.AddNode(n);
    }

}
