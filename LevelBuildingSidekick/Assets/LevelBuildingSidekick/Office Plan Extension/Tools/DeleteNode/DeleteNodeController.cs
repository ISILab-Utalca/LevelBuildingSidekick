using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNodeController : ToolController
{
    public DeleteNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new DeleteNodeView(this);
        OnButtonClick.AddListener(() => { IsActive = !IsActive;}); //Debug.Log(IsActive); });
    }


    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;
        if(graph.SelectedNode != null)
        {
            NodeController n = graph.SelectedNode;
            Debug.Log(graph.SelectedNode);
            graph.Nodes.Remove(graph.SelectedNode);
            graph.SelectedNode = null;
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
