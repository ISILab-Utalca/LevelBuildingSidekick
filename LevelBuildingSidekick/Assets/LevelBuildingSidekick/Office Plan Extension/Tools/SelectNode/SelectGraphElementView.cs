using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGraphElementView : ToolView
{
    Texture2D active;
    Texture2D unnactive;
    public SelectGraphElementView(Controller controller) : base(controller)
    {
        active = Resources.Load("Icons/Icon2_Select") as Texture2D;
        unnactive = Resources.Load("Icons/Icon1_Select") as Texture2D;
    }

    public override void DrawInToolkit()
    {
        var data = Controller.Data as SelectGraphElementData;
        var controller = Controller as SelectGraphElementController;
        
        var t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.Switch();
        }
    }

    public override void Draw2D()
    {
        // -> Feedback of selection
    }
}
