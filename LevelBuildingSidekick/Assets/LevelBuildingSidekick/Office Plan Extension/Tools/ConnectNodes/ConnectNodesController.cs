using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class ConnectNodesController : ToolController
{
    public Vector2 InitialPos { get; set; }

    LBSNodeController firstNode;
    public ConnectNodesController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new ConnectNodesView(this);
        IsActive = false;
    }

    public override void Action(LevelRepresentationController level)
    {
        if(firstNode == null)
        {
            return;
        }
        LBSGraphController graph = level as LBSGraphController;

        LBSNodeController n = graph.GetNodeAt(Event.current.mousePosition);

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

        EdgeData edge = new EdgeData(firstNode.Data as LBSNodeData, n.Data as LBSNodeData);
        graph.AddEdge(edge);

        firstNode = null;
    }

    public override void LoadData()
    {
    }

    bool SelectNode(LevelRepresentationController level)
    {
        LBSGraphController graph = level as LBSGraphController;

        LBSNodeController n = graph.GetNodeAt(Event.current.mousePosition);
        if (n == null)
        {
            return false;
        }
        graph.SelectedNode = n;
        firstNode = n;
        InitialPos = Event.current.mousePosition;
        CurrentPos = Event.current.mousePosition;
        return true;
    }

    // Update is called once per frame
    public override void Update() 
    {

    }

    public override void OnMouseDown(Vector2 position)
    {
        if(SelectNode(Toolkit.Level))
        {
            (View as ConnectNodesView).ShowLine(true);
        }
    }

    public override void OnMouseUp(Vector2 position)
    {
        (View as ConnectNodesView).ShowLine(false);
        Action(Toolkit.Level);
    }

    public override void OnMouseDrag(Vector2 position)
    {
        CurrentPos = position;
    }
}
