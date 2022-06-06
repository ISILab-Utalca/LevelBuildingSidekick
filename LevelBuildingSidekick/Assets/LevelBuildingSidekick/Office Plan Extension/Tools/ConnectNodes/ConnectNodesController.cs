using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class ConnectNodesController : ToolController
{
    public Vector2 InitialPos { get; set; }
    public Vector2 CurrentPos { get; set; }

    NodeController firstNode;
    public ConnectNodesController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new ConnectNodesView(this);
        IsActive = false;
        OnButtonClick.AddListener(() => { IsActive = !IsActive; });
    }

    public override void Action(LevelRepresentationController level)
    {
        if(firstNode == null)
        {
            return;
        }
        GraphController graph = level as GraphController;

        NodeController n = graph.GetNodeAt(Event.current.mousePosition);

        InitialPos = Vector2.zero;
        CurrentPos = Vector2.zero;
        if (n == null)
        {
            //Debug.Log("NULL");
            return;
        }
        if(n.Equals(firstNode)) // -> Can not connect to itself
        {
            //Debug.Log("SameNode");
            return;
        }
        if(graph.GetEdge(firstNode,n) != null) // -> Cannot create redundant edges
        {
            //Debug.Log("AlreadyConnected");
            return;
        }

        EdgeData edge = ScriptableObject.CreateInstance<EdgeData>();
        edge.node1 = firstNode.Data as NodeData;
        edge.node2 = n.Data as NodeData;
        if(graph.AddEdge(edge))
        {
            firstNode.neighbors.Add(n);
            n.neighbors.Add(firstNode);
        }

        firstNode = null;
    }

    public override void LoadData()
    {
    }

    void SelectNode(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        NodeController n = graph.GetNodeAt(Event.current.mousePosition);
        if (n == null)
        {
            return;
        }
        graph.SelectedNode = n;
        firstNode = n;
        InitialPos = Event.current.mousePosition;
        CurrentPos = Event.current.mousePosition;
    }

    // Update is called once per frame
    public override void Update()
    {
        if(IsActive)
        {
            Event e = Event.current;
            if (e.button == 0 && e.type.Equals(EventType.MouseDown))
            {
                SelectNode(Toolkit.Level);
                //Debug.Log("Down");
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseDrag))
            {
                CurrentPos = e.mousePosition;
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseUp))
            {
                Action(Toolkit.Level);
                IsActive = false;
            }
        }
    }
}
