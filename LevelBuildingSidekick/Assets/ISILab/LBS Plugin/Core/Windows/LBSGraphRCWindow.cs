using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LBS.Transformers;
using System;
using UnityEditor.Overlays;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;

namespace LBS.Windows
{
    public class LBSGraphRCWindow : GenericGraphWindow , ISupportsOverlays// RC => roomcharacteristics
    {
        private LBSGraphRCWindow() { }

        [MenuItem("ISILab/LBS plugin/GraphRC window")] // graphRC means Graph Room Characteristics
        [LBSWindow("GraphRC window")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSGraphRCWindow>();
            wnd.titleContent = new GUIContent("GraphRC window");
        }

        public override void OnCreateGUI()
        {
           
        }

        public override void OnInitPanel()
        {
            //throw new NotImplementedException();
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)
            var graphData = data.GetRepresentation<LBSGraphData>();
            controllers.Add(new LBSGraphRCController(MainView, graphData));
        }
    }
}
