using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New tags list",menuName = "ISILab/LBS plugin/Tags List")]
public class Tags_SO : ScriptableObject // cambiar nombre (!)
{
    [SerializeField]
    private readonly List<string> basics = new List<string>() {"Floor","Ceiling","Wall","Door","Prop"}; // podria ser estructura en vez de string (?)
    [SerializeField]
    private List<string> others = new List<string>();

    public List<string> Basics => new List<string>(basics);
    public List<string> Others => new List<string>(others);

    public void SetTag(int n, string value)
    {
        others[n] = value;
    }
    
    public void AddTag(string tag = "")
    {
        others.Add(tag);
    }

    public void RemoveLast()
    {
        if(others.Count > 0)
        {
            others.Remove(others.Last());
        }
    }
}


[CustomEditor(typeof(Tags_SO))]
public class Tags_SO_Editor : Editor // cambiar nombre (!)
{
    public override void OnInspectorGUI()
    {
        var t = target as Tags_SO;

        if (t == null)
            return;

        serializedObject.Update();
        EditorGUILayout.LabelField("Tags",EditorStyles.boldLabel);
        for (int i = 0; i < t.Basics.Count; i++)
        {
            var tag = t.Basics[i];
            ReadOnlyTextField("Tag " + i, tag);
        }
        for (int i = 0; i < t.Others.Count; i++)
        {
            var tag = t.Others[i];
            var v = EditorGUILayout.TextField(tag);
            t.SetTag(i,v);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
        {
            t.AddTag("");
        }
        if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
        {
            t.RemoveLast();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    void ReadOnlyTextField(string label, string text)
    {
        EditorGUILayout.BeginHorizontal();
        {
            //EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
    }
}
