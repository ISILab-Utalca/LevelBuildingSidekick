using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Newtonsoft.Json;

public class LBSEdgeView : GraphElement
{
    public LBSEdgeData Edge;
    public LBSEdgeView(Controller controller) //: base(controller)
    {
        capabilities |= Capabilities.Selectable | Capabilities.Deletable;
    }

    public void Draw2D() // solo necesita un contructor del visual element
    {
        //EdgeData c = edge;

        //Debug.Log("P1: " + c.Node1.Position + " - P2: " + c.Node2.Position);
        //Debug.Log("C1: " + c.Node1.Centroid + " - C2: " + c.Node2.Centroid);
        //Debug.Log("A1: " + pos1 + " - A2: " + pos2);

        //Handles.BeginGUI();
        //Handles.DrawAAPolyLine(c.Thickness, pos1, pos2);
        //Handles.EndGUI();
    }
}

