using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateNodeView : ToolView
{
    string active;
    string unnactive;
    public CreateNodeView(Controller controller) : base(controller)
    {
        active = "Node";

        unnactive = "!Node";
        
    }

    public override void DisplayInToolkit()
    {
        DrawInToolkit();
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as CreateNodeData;
        var controller = Controller as CreateNodeController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
        string t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.Switch();
        }
    }

    public override void Draw2D()
    {
    }
}
