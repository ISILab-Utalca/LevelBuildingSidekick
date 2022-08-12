using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;

public class EdgeController : Controller
{
    public LBSNodeController Node1 { get; set; }
    public LBSNodeController Node2 { get; set; }
    public float Thickness
    {
        get
        {
            return (Data as LBSEdgeData).thikness;
        }
    }

    public EdgeController(Data data) : base(data)
    {

    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
    }

    internal bool DoesConnect(LBSNodeController n1, LBSNodeController n2)
    {
        LBSEdgeData d = Data as LBSEdgeData;
        return ((n1.Data.Equals(d.firstNode) && n2.Data.Equals(d.secondNode)) || (n2.Data.Equals(d.firstNode) && n1.Data.Equals(d.secondNode)));
    }

    internal bool Contains(LBSNodeController node)
    {
        LBSEdgeData d = Data as LBSEdgeData;
        return (node.Data.Equals(d.firstNode) || node.Data.Equals(d.secondNode));
    }
}
