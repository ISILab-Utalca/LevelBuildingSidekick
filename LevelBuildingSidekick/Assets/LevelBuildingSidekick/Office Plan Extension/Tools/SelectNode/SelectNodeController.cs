using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class SelectNodeController : ToolController
{
    bool waiting;
    //public static NodeController SelectedNode { get; set; }
    public SelectNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new SelectNodeView(this);
    }

    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        NodeController n = graph.GetNodeAt(Event.current.mousePosition);
        if(n!=null)
        {
            graph.SelectedNode = n;
        }
    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
        Event e = Event.current;
        if (e.button == 0 && e.type.Equals(EventType.MouseDown))
        {
            //Debug.Log("Down");
            waiting = true;
        }
        if (waiting && (e.button == 0 && e.type.Equals(EventType.MouseUp)))
        {
            Action(Toolkit.Level);
            waiting = false;
        }
    }
}
