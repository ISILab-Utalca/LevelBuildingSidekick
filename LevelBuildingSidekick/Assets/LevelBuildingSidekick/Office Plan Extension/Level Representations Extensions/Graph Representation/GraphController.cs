using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class GraphController : LevelRepresentationController
{
    public List<NodeController> Nodes { get; set; }
    public List<EdgeController> Edges { get; set; }
    public GraphController(Data data) : base(data)
    {
        View = new GraphView(this);
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

    public override void Update()
    {
        Toolkit.Update();
    }
}
