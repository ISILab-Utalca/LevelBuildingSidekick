using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        // propiedad que se pueda cambiar para que el usuario pueda dejar
        // de ver esta pantalla al inicio del proyecto si lo necesita
        //if (false)
        //    return;

        var onStart = SessionState.GetBool("start",true);
        if(onStart)
        {
            EditorApplication.update += Start;
            SessionState.SetBool("start",false);
        }
    }

    private static void Start()
    {
        LBSStartWindow.ShowWindow();
        //Debug.Log("hello world");
        EditorApplication.update -= Start;
    }
}
