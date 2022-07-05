using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNodeView : ToolView
{
    Texture2D active;
    Texture2D unnactive;
    public MoveNodeView(Controller controller) : base(controller)
    {
        active = Resources.Load("Icons/Icon2_Move") as Texture2D;
        unnactive = Resources.Load("Icons/Icon1_Move") as Texture2D;
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as MoveNodeData;
        var controller = Controller as MoveNodeController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);

        var t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.Switch();
        }
    }

    public override void Draw2D()
    {
    }
}
