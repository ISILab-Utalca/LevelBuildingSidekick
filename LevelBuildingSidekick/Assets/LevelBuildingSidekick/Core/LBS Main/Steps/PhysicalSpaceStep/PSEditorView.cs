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
            Window.draw = Draw;
            //Debug.Log("D: " + (Controller.Data as PSEditorData));

        }

        public override void Display()
        {
            Window.titleContent.text = (Controller.Data as PSEditorData).WindowName;
            Window.Show();
        }

        public override void Draw()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            //Debug.Log((Controller as PSEditorController).Level);
            //Debug.Log((Controller as PSEditorController).Level.View);
            (Controller as PSEditorController).Level.View.Display();
            EditorGUILayout.EndScrollView();
        }


    }
}

