using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ScriptableObjectReferenceAttribute))]
public class StSDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as ScriptableObjectReferenceAttribute;
        if (att == null)
        {
            GUILayout.Label("[Error StS Atribute]");
            return;
        }


        var list = att.SOs.Select(so => so.name).ToList();
        var v = property.stringValue;

        var n = 0;
        
        if (v != default(string))
            n = list.IndexOf(v);

        EditorGUI.BeginProperty(position, label, property);

        var t = EditorGUI.Popup(position, att.type.Name, n, list.ToArray());
        //var t = EditorGUILayout.Popup(n, list.ToArray());

        EditorGUI.EndProperty();

        if (t < list.Count && t >= 0)
        {
            property.stringValue = list[t];
        }
        else
        {
            property.stringValue = list[0];
        }
    }
}