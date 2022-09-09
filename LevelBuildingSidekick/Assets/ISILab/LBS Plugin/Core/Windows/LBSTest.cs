using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBS.Windows
{
    public class LBSTest : GenericGraphWindow // es necesario una clase "LBSTest" que se va a hacer con ella (!!)
    {

        [MenuItem("ISILab/LBS plugin/Test step.../test (nombre temporal)")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSTest>();
            wnd.titleContent = new GUIContent("Test window");

            Debug.Log(wnd); // son vitales...
            wnd.position = wnd.position; // VITALES!!!!
        }

        public override void OnCreateGUI()
        {
            //throw new System.NotImplementedException();
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