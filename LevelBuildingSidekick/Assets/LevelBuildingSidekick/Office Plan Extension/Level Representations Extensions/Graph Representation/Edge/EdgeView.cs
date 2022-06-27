using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

public class EdgeView : View
{
    public EdgeView(Controller controller) : base(controller)
    {
    }

    public override void Draw2D()
    {
        EdgeController c = Controller as EdgeController;
        EdgeData d = c.Data as EdgeData;

        float r1 = d.node1.radius;
        float r2 = d.node2.radius;

        Vector2 pos1 = c.Node1.GetAnchor(c.Node2.Centroid);
        Vector2 pos2 = c.Node2.GetAnchor(c.Node1.Centroid);

        //Debug.Log("P1: " + c.Node1.Position + " - P2: " + c.Node2.Position);
        //Debug.Log("C1: " + c.Node1.Centroid + " - C2: " + c.Node2.Centroid);
        //Debug.Log("A1: " + pos1 + " - A2: " + pos2);

        Handles.BeginGUI();
        Handles.DrawAAPolyLine(d.thikness, pos1, pos2);
        Handles.EndGUI();
    }
}
