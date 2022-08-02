using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ConnectNodesView : ToolView
{
    Texture2D active;
    Texture2D unnactive;

    private bool showLine;
    float thikness;
    public ConnectNodesView(Controller controller) : base(controller)
    {
        active = Resources.Load("Icons/Icon2_Edge") as Texture2D;
        unnactive = Resources.Load("Icons/Icon1_Edge") as Texture2D;
        
        var data = controller.Data as ConnectNodesData;
        thikness = data.thikness;
}

    public override void DrawInToolkit()
    {
        var data = Controller.Data as ConnectNodesData;
        var controller = Controller as ConnectNodesController;

        var texture = controller.IsActive ? active : unnactive; 
        if (GUILayout.Button(texture))
        {
            controller.Switch();
        }
    }

    public override void Draw2D() 
    {
        if (!showLine)
            return;

        ConnectNodesController c = Controller as ConnectNodesController;
        if(c.InitialPos == Vector2.zero && c.CurrentPos == Vector2.zero)
        {
            return;
        }

        Handles.BeginGUI();
        Handles.DrawAAPolyLine(thikness, c.InitialPos, c.CurrentPos);
        Handles.EndGUI();
    }

    public void ShowLine(bool v)
    {
        showLine = v;
    }
}
