using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LBS.Windows
{
    public class LBSQuest : GenericGraphWindow
    {
        [MenuItem("ISILab/LBS plugin/Quest step.../quest (nombre temporal)")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSQuest>();
            wnd.titleContent = new GUIContent("Quest window");

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