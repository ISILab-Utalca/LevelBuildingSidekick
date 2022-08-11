using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;

public class ToolkitView : VisualElement
{
    //ToolkitOverlay toolkit;

    public ToolkitView(Controller controller) //: base(controller)
    {
        //var toolkit = new ToolkitOverlay();
        //toolkit = draw;
        //Debug.Log("Hi: " + draw);
        //Debug.Log((controller as ToolkitController).ToolControllers.Count);
        /*if (toolkit != null)
        {
            toolkit.draw = DrawEditor;
        }*/
    }

    public void DrawEditor()
    {
        /*
        //GUILayout.Label("Toolkit");
        var controller = Controller as ToolkitController;
        //controller.Update();
        //Debug.Log("V: " + controller.Tools.Count);
        foreach (ToolController t in controller.Tools)
        {
            (t.View as ToolView).DrawInToolkit();
        }
        */
    }

    public void Draw2D()
    {
        /*
        var controller = Controller as ToolkitController;
        foreach (ToolController t in controller.Tools)
        {
            (t.View as View).Draw2D();
        }
        */
    }

}
