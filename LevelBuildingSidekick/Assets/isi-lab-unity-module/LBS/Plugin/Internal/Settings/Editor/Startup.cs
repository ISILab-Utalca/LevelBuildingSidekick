using ISILab.LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS
{
    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {

            var onStart = SessionState.GetBool("start", true);
            if (onStart)
            {
                EditorApplication.update += Start;
                SessionState.SetBool("start", false);
            }
        }

        private static void Start()
        {
            // TODO: open a window that opens at the beginning of the use of unity

            SettingsEditor.SearchSettingsInstance();
            EditorApplication.update -= Start;
        }
    }
}