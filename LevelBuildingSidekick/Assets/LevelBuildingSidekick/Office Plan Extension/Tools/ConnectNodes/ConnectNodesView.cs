using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConnectNodesView : ToolView
{
    Texture2D active;
    Texture2D unnactive;
    float thikness;
    public ConnectNodesView(Controller controller) : base(controller)
    {
        active = Resources.Load("Icons/Icon2_Edge") as Texture2D;
        unnactive = Resources.Load("Icons/Icon1_Edge") as Texture2D;
        thikness = 2;
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as ConnectNodesData;
        var controller = Controller as ConnectNodesController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
        var t = controller.IsActive ? active : unnactive; 
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
