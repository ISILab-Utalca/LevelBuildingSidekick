using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class ColorExtensions
{
    public static Color RandomColor(this Color color)
    {
        color = new Color(
            Random.Range(0f, 255f) / 255f,
            Random.Range(0f, 255f) / 255f,
            Random.Range(0f, 255f) / 255f);
        return color;
    }

    public static Color RandomGrayScale(this Color color)
    {
        var gray = Random.Range(0f, 255f) / 255f;
        color = new Color(gray, gray, gray);
        return color;
    }

    public static Color Inverse(this Color color)
    {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b);
    }

    public static SerializableColor ToSerializable(this Color color)
    {
        return new SerializableColor(color.r, color.g, color.b, color.a);
    }
}


[System.Serializable]
public struct SerializableColor
{
    public float r, g, b, a;

    public SerializableColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, a);
    }
}

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
        var color = EditorGUI.ColorField(position, new Color(r,g,b,a));

        property.FindPropertyRelative("r").floatValue = color.r;
        property.FindPropertyRelative("g").floatValue = color.g;
        property.FindPropertyRelative("b").floatValue = color.b;
        property.FindPropertyRelative("a").floatValue = color.a;

        EditorGUI.EndProperty();
    }
}