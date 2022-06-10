using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

public class NodeWindow : EditorWindow
{
    public Controller controller { get; set; }
    Vector2 scrollPos;
    private void OnEnable()
    {
    }

    private void OnInspectorUpdate()
    {
        //Repaint();
    }

    private void OnGUI()
    {
        if(controller.Data == null) 
        {
            GUILayout.Label("None Selected");
            return;
        }
        controller.View.DrawEditor();
    }

    private void OnDisable()
    {
        //base.SaveChanges();
        //DestroyImmediate(this);
    }
}
