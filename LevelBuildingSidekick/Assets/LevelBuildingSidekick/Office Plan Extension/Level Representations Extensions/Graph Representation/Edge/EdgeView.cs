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

    public override void Display()
    {
        Draw();
    }

    public override void Draw()
    {
        EdgeController c = Controller as EdgeController;
        EdgeData d = c.Data as EdgeData;

        float r1 = d.node1.Radius;
        float r2 = d.node2.Radius;

        Vector2 pos1 = d.node1.Position;
        Vector2 pos2 = d.node2.Position;

        if(Mathf.Abs(pos1.x - pos2.x) > Mathf.Abs(pos1.y - pos2.y))
        {
            pos1.y += r1;
            pos2.y += r2;
            if(pos1.x < pos2.x)
            {
                pos1.x += r1 * 2;
            }
            else
            {
                pos2.x += r2 * 2;
            }
        }
        else
        {
            pos1.x += r1;
            pos2.x += r2;
            if (pos1.y < pos2.y)
            {
                pos1.y += r1 * 2;
            }
            else
            {
                pos2.y += r2 * 2;
            }
        }

        Handles.BeginGUI();
        Handles.DrawAAPolyLine(d.thikness, pos1, pos2);
        Handles.EndGUI();
    }
}
