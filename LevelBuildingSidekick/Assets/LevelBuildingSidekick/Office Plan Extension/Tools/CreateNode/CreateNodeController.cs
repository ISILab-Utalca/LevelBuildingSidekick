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
    }

    public override void Switch()
    {
        //Debug.Log(IsActive);
        base.Switch();
        //Debug.Log(IsActive);
        waiting = false;
    }

    public override void Action(LevelRepresentationController level)
    {
        //Debug.Log("New node");
        
        GraphController graph = level as GraphController;
        //Debug.Log(level + " - " + graph);
        if (graph == null)
        {
            Debug.Log("NULL Graph");
            return;
        }


        //NodeData node = ScriptableObject.CreateInstance<NodeData>();
        NodeData node = new NodeData();


        node.position = new Vector2Int((int)(Event.current.mousePosition.x - node.radius), (int)(Event.current.mousePosition.y - node.radius));
        node.room = new LevelBuildingSidekick.Schema.RoomCharacteristics();
        int index = graph.Nodes.Count;
        node.room.label = "Node: " + index.ToString();
        while(!graph.AddNode(node))
        {
            index++;
            node.room.label = "Node: " + index.ToString();
        }
        graph.AddNode(node);
        //Debug.Log("New node: " + node.room + " Node Count: " + graph.Nodes.Count);

    }


    public override void LoadData()
    {
    }

    public override void Update()
    {
        //Debug.Log(IsActive);
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
                //Debug.Log("Hi");
                Action(Toolkit.Level);
                waiting = false;
                IsActive = false;
            }
        }
        
    }
}
