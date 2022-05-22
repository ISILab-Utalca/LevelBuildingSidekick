using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class NodeWindow : EditorWindow
{
    public NodeData Data { get; set; }
    Vector2 scrollPos;
    private void OnEnable()
    {
        minSize = new Vector2(100, 100);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if(Data == null)
        {
            GUILayout.Label("NoNode");
            return;
        }
        var editor = Editor.CreateEditor(Data);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        editor.DrawDefaultInspector();
        EditorGUILayout.EndScrollView();
    }

    private void OnDisable()
    {
        base.SaveChanges();
        //DestroyImmediate(this);
    }
}
