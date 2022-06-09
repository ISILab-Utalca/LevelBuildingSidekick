using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class DeleteNodeController : ToolController
{
    public DeleteNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new DeleteNodeView(this);
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
