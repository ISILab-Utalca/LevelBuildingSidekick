using LBS.Settings;
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
        // aqui podemos abrir una ventana que se abra al inicio del uso de unity
        // y que te lleve a novedades y o tutoriales de la herramienta

        // busca y setea la instancia de "LBS Settings" en su singleton
        SettingsEditor.SearchSettingsInstance();

        //var storage = LBSAssetsStorage.Instance;

        EditorApplication.update -= Start;
    }
}
