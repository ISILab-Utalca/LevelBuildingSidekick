using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class CreateNodeController : ToolController
{
    bool waiting;
    public CreateNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new CreateNodeView(this);
        OnButtonClick.AddListener(() => { IsActive = !IsActive; waiting = false; }); //Debug.Log(IsActive); });
        IsActive = false;
    }

    public override void Action(LevelRepresentationController level)
    {
        //Debug.Log("New node");
        
        GraphController graph = level as GraphController;
        //Debug.Log(level + " - " + graph);
        if (graph == null)
        {
            return;
        }


        NodeData node = ScriptableObject.CreateInstance<NodeData>();


        node.position = new Vector2Int((int)(Event.current.mousePosition.x - node.Radius), (int)(Event.current.mousePosition.y - node.Radius));
        node.label = "Node: " + graph.Nodes.Count.ToString();
        graph.AddNode(node);
        //Debug.Log("New node: " + node.label + " Node Count: " + graph.Nodes.Count);

    }


    public override void LoadData()
    {
    }

    public override void Update()
    {
        if(IsActive)
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
                IsActive = false;
            }
        }
        
    }
}
