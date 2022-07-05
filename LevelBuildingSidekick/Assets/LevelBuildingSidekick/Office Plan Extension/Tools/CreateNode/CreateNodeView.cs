using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateNodeView : ToolView
{
    Texture2D active;
    Texture2D unnactive;
    public CreateNodeView(Controller controller) : base(controller)
    {
        active = Resources.Load("Icons/Icon2_Node") as Texture2D;

        unnactive = Resources.Load("Icons/Icon1_Node") as Texture2D;
        
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as CreateNodeData;
        var controller = Controller as CreateNodeController;
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
