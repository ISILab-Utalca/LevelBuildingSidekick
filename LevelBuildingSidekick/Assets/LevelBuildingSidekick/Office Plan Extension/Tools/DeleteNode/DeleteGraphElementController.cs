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
        if(graph.SelectedNode != null)
        {
            NodeController n = graph.SelectedNode;
            //Debug.Log(graph.SelectedNode);
            graph.RemoveNode(graph.SelectedNode);
            Object.DestroyImmediate(n.Data);
            IsActive = false;
        }
        else if(graph.SelectedEdge != null)
        {
            EdgeController e = graph.SelectedEdge;
            graph.RemoveEdge(e);
            Object.DestroyImmediate(e.Data);
            IsActive = false;
        }
    }

    public override void LoadData()
    {

    }

    public override void Update()
    {
        if (IsActive)
        {
            Action(Toolkit.Level);
        }
    }
}
