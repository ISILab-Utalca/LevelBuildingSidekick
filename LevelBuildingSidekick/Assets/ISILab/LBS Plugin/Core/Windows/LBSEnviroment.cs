using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LBSEnviroment : LBSEditorWindow
{
    [MenuItem("LBS/Enviroment step.../enviroment (nombre temporal)(la de los colorinches y luces)")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<LBSEnviroment>();
        wnd.titleContent = new GUIContent("Enviroment window");

        Debug.Log(wnd); // son vitales...
        wnd.position = wnd.position; // VITALES!!!!
    }

    public override void OnCreateGUI()
    {
        //throw new System.NotImplementedException();
    }

}
