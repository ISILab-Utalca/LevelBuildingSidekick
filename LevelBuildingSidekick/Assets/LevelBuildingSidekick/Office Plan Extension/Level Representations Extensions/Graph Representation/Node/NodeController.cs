using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;

public class NodeController : Controller
{
    public NodeController(Data data) : base(data)
    {
        View = new NodeView(this);
    }

    public override void LoadData()
    {
        //throw new System.NotImplementedException();
    }

    public override void Update()
    {
    }
}
