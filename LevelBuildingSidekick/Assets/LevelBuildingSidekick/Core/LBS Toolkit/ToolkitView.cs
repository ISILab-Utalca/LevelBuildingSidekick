using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;

public class ToolkitView : View
{

    public ToolkitView(Controller controller) : base(controller)
    {
        ToolkitOverlay.draw = Draw;
    }

    public override void Display()
    {
        //Draw();
    }

    public override void Draw()
    {
        //GUILayout.Label("Toolkit");
        var controller = Controller as ToolkitController;
        //controller.Update();
        foreach(ToolController t in controller.ToolControllers)
        {
            t.View.Display();
        }
    }

}
