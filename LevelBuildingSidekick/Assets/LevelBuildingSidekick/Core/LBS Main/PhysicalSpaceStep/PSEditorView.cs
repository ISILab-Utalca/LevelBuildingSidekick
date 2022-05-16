using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class PSEditorView : View
    {
        PSEWindow Window { get; set; }
        Vector2 scrollPosition;

        public PSEditorView(Controller controller):base(controller)
        {
            scrollPosition = Vector2.zero;
            Window = new PSEWindow();
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
            (Controller as PSEditorController).level.View.Display();
            EditorGUILayout.EndScrollView();
        }
    }

    public class PSEWindow: EditorWindow
    {
        public System.Action draw;
        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            draw?.Invoke();
        }
    }
}

