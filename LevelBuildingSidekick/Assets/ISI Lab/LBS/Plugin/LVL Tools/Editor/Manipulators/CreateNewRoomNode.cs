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
    private MainView mainView;

    private string prefix = "";
    private string postfix = "";

    public CreateNewRoomNode(/*string prefix = "", string postfix = ""*/) : base() 
    {
        this.prefix = "Node: ";
    }

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<GraphModule<RoomNode>>();
        this.mainView = view;
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
        OnManipulationStart?.Invoke();
        var pos = mainView.FixPos(e.localMousePosition);
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
        module.AddNode(n);
        this.OnManipulationEnd?.Invoke();
    }

}
