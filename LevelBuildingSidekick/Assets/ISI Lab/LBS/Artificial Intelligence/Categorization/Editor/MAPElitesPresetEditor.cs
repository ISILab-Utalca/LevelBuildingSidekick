using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MAPElitesPreset))]
public class MAPElitesPresetEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        //return base.CreateInspectorGUI();
        return new MAPElitesPresetVE(target);
    }

    /*public override void OnInspectorGUI()
    {
        new MAPElitesPressetVE(target);
        //base.OnInspectorGUI();
    }*/

}
