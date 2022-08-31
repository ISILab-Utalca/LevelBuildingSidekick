using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LBSTest : LBSEditorWindow // es necesario una clase "LBSTest" que se va a hacer con ella (!!)
{

    [MenuItem("ISILab/LBS plugin/Test step.../test (nombre temporal)")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<LBSEnviroment>();
        wnd.titleContent = new GUIContent("Test window");

        Debug.Log(wnd); // son vitales...
        wnd.position = wnd.position; // VITALES!!!!
    }

    public override void OnCreateGUI()
    {
        //throw new System.NotImplementedException();
    }
}
