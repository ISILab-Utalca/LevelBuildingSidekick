using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

public class GraphView : LevelRepresentationView
{
    public GraphView(Controller controller) : base(controller)
    {
    }

    public override void Draw()
    {
        //Debug.Log("Graph View");
        var controller = Controller as GraphController;

        foreach(NodeController n in controller.Nodes)
        {
            n.View.Display();
        }
        foreach(EdgeController e in controller.Edges)
        {
            e.View.Display();
        }

        base.Draw();
    }
}
