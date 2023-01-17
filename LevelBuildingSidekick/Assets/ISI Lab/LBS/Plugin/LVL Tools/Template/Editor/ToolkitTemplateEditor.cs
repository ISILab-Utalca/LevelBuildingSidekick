using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolkitTemplate))]
public class ToolkitTemplateEditor : Editor
{
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var template = (ToolkitTemplate)target;

        if (GUILayout.Button("Set as SchemaInterior"))
        {
            SchemaInterior(template);
        }

        if (GUILayout.Button("Set as GraphInterior"))
        {
            GraphInterior(template);
        }

        if (GUILayout.Button("Set as Exterior"))
        {
            Exterior(template);
        }

        if (GUILayout.Button("Set as Population"))
        {
            Population(template);
        }
    }

    private void SchemaInterior(ToolkitTemplate template)
    {

    }

    private void GraphInterior(ToolkitTemplate template)
    {

    }

    private void Exterior(ToolkitTemplate template)
    {

    }

    private void Population(ToolkitTemplate template)
    {

    }

}
