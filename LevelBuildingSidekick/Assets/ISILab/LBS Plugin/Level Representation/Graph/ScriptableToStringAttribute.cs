using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public class ScriptableToStringAttribute : PropertyAttribute
{
    public List<ScriptableObject> SOs;

    public ScriptableToStringAttribute(Type type)
    {
        SOs = DirectoryTools.GetScriptables(type);
    }
}

[CustomPropertyDrawer(typeof(ScriptableToStringAttribute))]
public class StSDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as ScriptableToStringAttribute;
        if(att == null)
        {
            GUILayout.Label("[Error StS Atribute]");
            return;
        }

        var list = att.SOs.Select(so => so.name).ToList();
        var v = property.stringValue;
        var n = list.IndexOf(v);
        var t = EditorGUI.Popup(position, n, list.ToArray());

        if(t < list.Count)
        {
            property.stringValue = list[t];
        }
        else
        {
            // Do nothing (??)
        }
    }
}
