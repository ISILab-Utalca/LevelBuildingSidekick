using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewNode<T> : LBSManipulator where T: LBSNode
{
    // ref Data
    private GraphModule<T> module;

    public override void InitData(ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = module as GraphModule<T>;
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

        var n = Activator.CreateInstance<T>();
        n.ID = name;
        n.Position = pos.ToInt();
        module.AddNode(n);
    }

}
