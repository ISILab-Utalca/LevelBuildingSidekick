using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LBSPopulation : LBSEditorWindow
{

    [MenuItem("LBS/Population step.../Population (nombre temporal)")]
    [LBSWindow("Population Window")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<LBSPopulation>();
        wnd.titleContent = new GUIContent("Population window");

        Debug.Log(wnd); // son vitales...
        wnd.position = wnd.position; // VITALES!!!!
    }

    public override void OnCreateGUI()
    {
        //throw new System.NotImplementedException();
    }
}
