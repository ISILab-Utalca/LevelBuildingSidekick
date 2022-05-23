using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class GraphController : LevelRepresentationController
{
    public List<NodeController> Nodes { get; set; }
    public NodeController SelectedNode { get; set; }
    public List<EdgeController> Edges { get; set; }
    public EdgeController SelectedEdge { get; set; }
    public GraphController(Data data) : base(data)
    {
        View = new GraphView(this);
    }

    public NodeController GetNodeAt(Vector2 pos)
    {
        foreach(NodeController n in Nodes)
        {
            if (n.GetRect().Contains(pos))
            {
                return n;
            }
        }
        return null;
    }

    internal void RemoveNode(NodeController node)
    {
        foreach (EdgeController e in Edges)
        {
            if (e.Contains(node))
            {
                RemoveEdge(e);
            }
        }
        Nodes.Remove(node);
        if(node.Equals(SelectedNode))
        {
            SelectedNode = null;    
        }
        GraphData d = Data as GraphData;
        d.nodes.Remove(node.Data as NodeData);

    }

    internal void RemoveEdge(EdgeController edge)
    { 
        Edges.Remove(edge);
        if (edge.Equals(SelectedEdge))
        {
            SelectedEdge = null;
        }
        GraphData d = Data as GraphData;
        d.edges.Remove(edge.Data as EdgeData);
    }

    public override void LoadData()
    {
        base.LoadData();

        var data = Data as GraphData;
        //Debug.Log("!: " + Data);

        Nodes = new List<NodeController>();

        foreach (NodeData n in data.nodes)
        {
            var node = Activator.CreateInstance(n.ControllerType, new object[] { n });
            if(node is NodeController)
            {
                Nodes.Add(node as NodeController);
                //Nodes[^1].Data = n;
            }
        }

        Edges = new List<EdgeController>();

        foreach (EdgeData e in data.edges)
        {
            var edge = Activator.CreateInstance(e.ControllerType, new object[] { e });
            if (edge is EdgeController)
            {
                Edges.Add(edge as EdgeController);
                //Edges[^1].Data = e;
            }
        }
    }

    public EdgeController GetEdge(NodeController n1, NodeController n2)
    {
        foreach(EdgeController e in Edges)
        {
            if(e.DoesConnect(n1,n2))
            {
                return e;
            }
        }
        return null;
    }

    internal bool AddNode(NodeData nodeData)
    {
        var node = Activator.CreateInstance(nodeData.ControllerType, new object[] { nodeData });
        if(node is NodeController)
        {
            Nodes.Add(node as NodeController);
            (Data as GraphData).nodes.Add(nodeData);
            return true;
        }
        return false;
    }


    internal bool AddEdge(EdgeData edgeData)
    {
        var edge = Activator.CreateInstance(edgeData.ControllerType, new object[] { edgeData });
        if (edge is EdgeController)
        {
            Edges.Add(edge as EdgeController);
            (Data as GraphData).edges.Add(edgeData);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        base.Update();
    }
}
