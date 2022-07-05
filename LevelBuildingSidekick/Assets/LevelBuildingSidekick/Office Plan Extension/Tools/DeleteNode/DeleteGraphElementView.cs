using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteGraphElementView : ToolView
{
    Texture2D active;
    Texture2D unnactive;
    public DeleteGraphElementView(Controller controller) : base(controller)
    {

        active = Resources.Load("Icons/Icon2_Delete") as Texture2D;
        unnactive = Resources.Load("Icons/Icon1_Delete") as Texture2D;
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as DeleteGraphElementData;
        var controller = Controller as DeleteGraphElementController;
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
