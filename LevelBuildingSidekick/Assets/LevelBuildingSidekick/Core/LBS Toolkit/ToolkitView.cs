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
        //Debug.Log((controller as ToolkitController).ToolControllers.Count);
        ToolkitOverlay.draw = DrawToolkit;
    }

    public void DrawToolkit()
    {
        //GUILayout.Label("Toolkit");
        var controller = Controller as ToolkitController;
        //controller.Update();
        foreach (ToolController t in controller.ToolControllers)
        {
            (t.View as ToolView).DisplayInToolkit();
        }
    }

    public override void Draw2D()
    {
        var controller = Controller as ToolkitController;
        foreach (ToolController t in controller.ToolControllers)
        {
            t.View.Draw2D();
        }
    }

}
