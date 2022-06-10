using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConnectNodesView : ToolView
{
    string active;
    string unnactive;
    float thikness;
    public ConnectNodesView(Controller controller) : base(controller)
    {
        active = "Edge";
        unnactive = "!Edge";
        thikness = 2;
    }

    public override void DisplayInToolkit()
    {
        DrawInToolkit();
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as ConnectNodesData;
        var controller = Controller as ConnectNodesController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
        string t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.Switch();
        }
    }

    public override void Draw2D() 
    {
        ConnectNodesController c = Controller as ConnectNodesController;
        if(c.InitialPos == Vector2.zero && c.CurrentPos == Vector2.zero)
        {
            return;
        }
        Handles.BeginGUI();
        Handles.DrawAAPolyLine(thikness, c.InitialPos, c.CurrentPos);
        Handles.EndGUI();
    }
}
