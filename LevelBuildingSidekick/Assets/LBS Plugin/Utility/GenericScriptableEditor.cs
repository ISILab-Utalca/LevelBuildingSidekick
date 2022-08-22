using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class GenericScriptableEditor : Editor
{
    private void OnEnable()
    {
    }


    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
        var prop = serializedObject.FindProperty("Data");
        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        return container;
    }
}
