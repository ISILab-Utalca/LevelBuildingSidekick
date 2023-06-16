using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomPropertyDrawer(typeof(SerializableColor))]
public class SerializableColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var r = property.FindPropertyRelative("r").floatValue;
        var g = property.FindPropertyRelative("g").floatValue;
        var b = property.FindPropertyRelative("b").floatValue;
        var a = property.FindPropertyRelative("a").floatValue;

        EditorGUI.BeginProperty(position, label, property);
        var color = EditorGUI.ColorField(position, new Color(r, g, b, a));

        property.FindPropertyRelative("r").floatValue = color.r;
        property.FindPropertyRelative("g").floatValue = color.g;
        property.FindPropertyRelative("b").floatValue = color.b;
        property.FindPropertyRelative("a").floatValue = color.a;

        EditorGUI.EndProperty();
    }
}