using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class PSEditorView : View
    {
        StepWindow Window { get; set; }
        Vector2 scrollPosition;

        public PSEditorView(Controller controller):base(controller)
        {
            scrollPosition = Vector2.zero;
            Window = EditorWindow.GetWindow<StepWindow>();
            //Window.minSize = Vector2.one* 100;
            //Window.Show();
            Window.draw = Draw2D;
            //Debug.Log("D: " + (Controller.Data as PSEditorData));

        }

        public override void Draw2D()
        {
            PSEditorController controller = Controller as PSEditorController;

            
            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
            //GUILayout.BeginArea(new Rect(0, 0, 1000, 1000));
            //Debug.Log((Controller as PSEditorController).Level);
            //Debug.Log((Controller as PSEditorController).Level.View);
            //GUILayout.BeginVertical();
            controller.Update();
            controller.Level.View.Draw2D();
            //GUILayout.EndVertical();
            //GUILayout.EndArea();
            //GUILayout.EndScrollView();
            
        }


    }
}

