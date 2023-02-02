using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(StringEnumAttribute))]
public class SEDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*
        var att = attribute as StringEnumAttribute;

        var all = new List<string>(att.DB.Basics).Concat(att.DB.Others).ToList();

        var db = all.Append("Add new...");
        var v = property.stringValue;
        var n = all.IndexOf(v);
        var t = EditorGUI.Popup(position, n, db.ToArray());

        if (t < all.Count)
        {
            property.stringValue = all.ToList()[t];
        }
        else
        {
            Selection.activeObject = att.DB;
            EditorGUIUtility.PingObject(att.DB);
            att.DB.AddTag("");

        }
        */
    }
}
