using LBS.View;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSPopulation : LBSEditorWindow
{
    private FreeStampView stampView; 

    [MenuItem("ISILab/LBS plugin/Population step.../Population (nombre temporal)")]
    [LBSWindow("Population Window")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<LBSPopulation>();
        wnd.titleContent = new GUIContent("Population window");

        //Debug.Log(wnd); // son vitales...
        //wnd.position = wnd.position; // VITALES!!!!
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("populationUXML");
        this.ImportStyleSheet("GraphWindow");

        stampView = root.Q<FreeStampView>();
    }

}
