using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MAPElitesPresset))]
public class MAPElitesPressetEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        //return base.CreateInspectorGUI();
        return new MAPElitesPressetVE(target);
    }

    /*public override void OnInspectorGUI()
    {
        new MAPElitesPressetVE(target);
        //base.OnInspectorGUI();
    }*/

}
