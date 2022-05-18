using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeController : ToolController
{
    Vector2 position;
    public CreateNodeController(Data data) : base(data)
    {
        position = Vector2.zero;
        View = new CreateNodeView(this);
    }

    public override void Action(LevelRepresentationController level)
    {
        //Debug.Log("New node");
        
        GraphController graph = level as GraphController;
        Debug.Log(level + " - " + graph);
        if (graph == null)
        {
            return;
        }


        NodeData node = new NodeData();
        node.Position = position;
        node.label = "Node: " + graph.Nodes.Count.ToString();
        graph.AddNode(node);
        Debug.Log("New node: " + node.label + " Node Count: " + graph.Nodes.Count);

    }

    public override void LoadData()
    {
    }

    public override void PrepareAction(LevelRepresentationController level)
    {
        (View as CreateNodeView).OnButtonClick.AddListener(() => { Action(level); });
    }

    public override void Update()
    {
        //if(Input.GetMouseButtonUp(0))
        //{
        //    position = Input.mousePosition;
        //}
    }
}
