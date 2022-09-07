using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LBS.Transformers;
using System;
using UnityEditor.Overlays;

namespace LevelBuildingSidekick.Graph
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
            //this.ImportUXML("MainView");
            //this.ImportStyleSheet("GraphWindow");

            //var generateBtn = root.Q<Button>("GenerateBtn");
            //generateBtn.clicked += () => { GenerateSchema(); };
        }

        public override void OnFocus()
        {
            var data = LBSController.CurrentLevel.data;
            controllers.ForEach(c => c.PopulateView(MainView));
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)
            var graphData = data.GetRepresentation<LBSGraphData>();
            controllers.Add(new LBSGraphRCController(MainView, graphData));
        }
    }
}
