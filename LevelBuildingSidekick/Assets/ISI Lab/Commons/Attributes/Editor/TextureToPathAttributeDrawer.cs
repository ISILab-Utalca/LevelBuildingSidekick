using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Start is called before the first frame update
[CustomPropertyDrawer(typeof(PathTextureAttribute))]
public class TextureToPathAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        var att = attribute as PathTextureAttribute;

        var path = property.stringValue;

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path); // esto puede lagear (!)

        att.texture = texture;
        att.texture = EditorGUI.ObjectField(position, "Layer Icon:", att.texture, typeof(Texture2D), false) as Texture2D;
        property.stringValue = AssetDatabase.GetAssetPath(att.texture);
    }
}