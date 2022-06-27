using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class SelectGraphElementController : ToolController
{
    bool waiting;
    //public static NodeController SelectedNode { get; set; }
    public SelectGraphElementController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new SelectGraphElementView(this);
    }

    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        if (graph == null)
        {
            return;
        } 
        var n = graph.GetNodeAt(Event.current.mousePosition);
        graph.SelectedNode = n;
        if (n == null)
        {
            graph.SelectedEdge = graph.GetEdgeAt(Event.current.mousePosition);
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
