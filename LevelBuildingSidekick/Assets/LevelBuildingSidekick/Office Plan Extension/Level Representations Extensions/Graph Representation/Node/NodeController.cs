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

    public Rect GetRect()
    {
        NodeData d = Data as NodeData;
        return new Rect(d.Position, d.Radius * 2 * Vector2.one);
    }

    public void Translate(Vector2 delta)
    {
        NodeData d = Data as NodeData;
        d.Position += delta;
    }
}
