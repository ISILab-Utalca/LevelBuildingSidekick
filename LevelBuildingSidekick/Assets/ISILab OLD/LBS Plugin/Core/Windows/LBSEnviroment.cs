using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBS.Windows
{
    public class LBSEnviroment : GenericLBSWindow
    {
        [MenuItem("ISILab/LBS plugin/Enviroment step.../enviroment (nombre temporal)(la de los colorinches y luces)")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSEnviroment>();
            wnd.titleContent = new GUIContent("Enviroment window");

            Debug.Log(wnd); // son vitales...
            wnd.position = wnd.position; // VITALES!!!!
        }


        public override void OnInitPanel()
        {
            throw new System.NotImplementedException();
        }

        public override void OnLoadControllers()
        {
            throw new System.NotImplementedException();
        }
    }
}