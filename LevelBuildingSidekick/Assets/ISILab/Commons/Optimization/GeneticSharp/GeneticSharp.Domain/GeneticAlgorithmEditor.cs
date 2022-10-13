using GeneticSharp.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// useless (???)

[CustomEditor(typeof(GeneticAlgorithm))]
public class GeneticAlgorithmEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var z = Editor.CreateEditor(target);
        var root = new VisualElement();
        root.Add(new IMGUIContainer(z.OnInspectorGUI));
        root.Add(new Label("Test"));
        return root;
    }
    

}
