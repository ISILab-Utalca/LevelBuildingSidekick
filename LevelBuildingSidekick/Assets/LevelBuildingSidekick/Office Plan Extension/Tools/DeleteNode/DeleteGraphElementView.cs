using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteGraphElementView : ToolView
{
    string active;
    string unnactive;
    public DeleteGraphElementView(Controller controller) : base(controller)
    {

        active = "Delete";
        unnactive = "!Delete";
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as DeleteGraphElementData;
        var controller = Controller as DeleteGraphElementController;
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
