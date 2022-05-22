using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNodeView : ToolView
{
    string active;
    string unnactive;
    public MoveNodeView(Controller controller) : base(controller)
    {
        active = "Moving";
        unnactive = "!Moving";
    }

    public override void Display()
    {
        Draw();
    }

    public override void Draw()
    {
        var data = Controller.Data as MoveNodeData;
        var controller = Controller as MoveNodeController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);

        string t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.OnButtonClick?.Invoke();
        }
    }
}
