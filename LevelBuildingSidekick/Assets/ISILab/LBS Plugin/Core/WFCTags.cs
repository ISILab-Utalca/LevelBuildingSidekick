using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System.Linq;
using UnityEditor;

public class WFCTags
{
    public LBSTagLists wfctags;

    List<string> lastTags;

    public AdjacencyMatrix adjacencies;


}

public class AdjacencyMatrix
{
    HashSet<object> xAxis;

    Dictionary<object, HashSet<object>> adjacencies;

    public AdjacencyMatrix(List<object> values)
    {
        xAxis = values.ToHashSet();
        foreach(var val in xAxis)
        {
            adjacencies.Add(val, new HashSet<object>());
        }
    }

    public void Add(object value)
    {
        if(xAxis.Add(value))
        {
            adjacencies.Add(value, new HashSet<object>());
            adjacencies[value].Add(value);
        }
    }

    public void Remove(object value)
    {
        if(xAxis.Remove(value))
        {
            adjacencies.Remove(value);
            foreach(var key in adjacencies.Keys)
            {
                adjacencies[key].Remove(value);
            }
        }
    }

    public void Update(object value1, object value2, bool adjacents)
    {
        if(!(xAxis.Contains(value1) && xAxis.Contains(value2)))
        {
            return;
        }

        if (adjacents)
        {
            adjacencies[value1].Add(value2);
            adjacencies[value2].Add(value1);
        }
        else
        {
            adjacencies[value1].Remove(value2);
            adjacencies[value2].Remove(value1);
        }
    }
}


public class AdjacencyMatrixEditor: PropertyDrawer
{
    Vector2 scroll;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);



        scroll = EditorGUILayout.BeginScrollView(scroll);



        EditorGUILayout.EndScrollView();
    }
}
