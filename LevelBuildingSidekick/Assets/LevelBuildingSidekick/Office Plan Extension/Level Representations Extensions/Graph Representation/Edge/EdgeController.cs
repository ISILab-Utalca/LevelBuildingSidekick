using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;

public class EdgeController : Controller
{
    public NodeController Node1 { get; set; }
    public NodeController Node2 { get; set; }

    public EdgeController(Data data) : base(data)
    {
        View = new EdgeView(this);
    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
    }

    internal bool DoesConnect(NodeController n1, NodeController n2)
    {
        EdgeData d = Data as EdgeData;
        return ((n1.Data.Equals(d.node1) && n2.Data.Equals(d.node2)) || (n2.Data.Equals(d.node1) && n1.Data.Equals(d.node2)));
    }

    internal bool Contains(NodeController node)
    {
        EdgeData d = Data as EdgeData;
        return (node.Data.Equals(d.node1) || node.Data.Equals(d.node2));
    }
}
