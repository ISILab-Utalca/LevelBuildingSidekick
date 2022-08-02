using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class DeleteGraphElementController : ToolController
{
    public DeleteGraphElementController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new DeleteGraphElementView(this);
    }

    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        var currentNode = graph.GetNodeAt(CurrentPos);
        if (currentNode != null)
        {
            graph.RemoveNode(currentNode);
            return;
        }

        var currentEdge = graph.GetEdgeAt(CurrentPos);
        if (currentEdge != null)
        {
            graph.RemoveEdge(currentEdge);
            return;
        }
    }

    public override void LoadData()
    {

    }

    public override void Update()
    {
        
    }

    public override void OnMouseDown(Vector2 position)
    {
        Action(Toolkit.Level);
    }
}
