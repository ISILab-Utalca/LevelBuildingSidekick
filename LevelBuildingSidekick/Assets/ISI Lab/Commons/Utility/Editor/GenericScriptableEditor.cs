using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;

public class GenericScriptableEditor : Editor
{
    private void OnEnable()
    {
    }


    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        return container;
    }
}
