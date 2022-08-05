using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class PSEditorView : View
    {
        GenericWindow Window { get; set; }

        public PSEditorView(Controller controller):base(controller)
        {

        }

        public override void Draw2D()
        {
            PSEditorController controller = Controller as PSEditorController;

            controller.Update();
            controller.LevelRepresentation.View.Draw2D();
            
        }

        public override void Display2DWindow()
        {
            PSEditorController controller = Controller as PSEditorController;
            controller.LevelRepresentation.View.Display2DWindow();
        }

        public override void DisplayInspectorWindow()
        {
            PSEditorController controller = Controller as PSEditorController;
            controller.LevelRepresentation.View.DisplayInspectorWindow();
        }



    }
}

