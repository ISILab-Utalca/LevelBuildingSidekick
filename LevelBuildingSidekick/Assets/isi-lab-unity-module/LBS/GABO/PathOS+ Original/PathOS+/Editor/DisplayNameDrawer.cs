using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PathOSDisplayNameAttribute))]
public class DisplayNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = (attribute as PathOSDisplayNameAttribute).displayName;
        EditorGUI.PropertyField(position, property, label);
    }
}
