using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.LBS.AI.Categorization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MAPElitesPreset))]
public class MAPElitesPresetEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        //return base.CreateInspectorGUI();
        var root = new MAPElitesPresetVE(target);


        return root;
    }

    public void Save()
    {
        EditorUtility.SetDirty(target);
    }

    private void OnDestroy()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }
}
