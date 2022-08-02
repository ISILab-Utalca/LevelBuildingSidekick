using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class SelectGraphElementController : ToolController
{
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

    public override void OnMouseDown(Vector2 position)
    {
    }

    public override void OnMouseUp(Vector2 position)
    {
        Action(Toolkit.Level);
    }

    public override void Update()
    {

    }
}
