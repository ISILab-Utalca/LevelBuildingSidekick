using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNodeView : ToolView
{
    string active;
    string unnactive;
    public DeleteNodeView(Controller controller) : base(controller)
    {

        active = "Delete";
        unnactive = "!Delete";
    }

    public override void Display()
    {
        Draw();
    }

    public override void Draw()
    {
        var data = Controller.Data as DeleteNodeData;
        var controller = Controller as DeleteNodeController;
        //controller.MousePosition = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
        
        string t = controller.IsActive ? active : unnactive;
        if (GUILayout.Button(t))
        {
            controller.OnButtonClick?.Invoke();
        }
    }
}
