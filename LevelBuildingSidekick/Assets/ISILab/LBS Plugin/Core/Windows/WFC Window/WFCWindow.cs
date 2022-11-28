using LBS.Generator;
using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace LBS.Windows
{
    // Esta clase usa:
    // MapData
    // WFCController
    public class WFCWindow : GenericGraphWindow, ISupportsOverlays
    {
        private WFCWindow() { }

        [MenuItem("ISILab/LBS plugin/WFC Window", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<WFCWindow>();
            window.titleContent = new GUIContent("WFC Window");
        }

        public override void OnInitPanel()
        {
            var generator = new PhysicStepGenerator();

            actions.Add(new System.Tuple<string, System.Action>(
                 "Generate 3D",
                 () => {
                     var data = LBSController.CurrentLevel.data;
                     generator.Init(data);
                     generator.Generate();
                 }
                 ));


        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data;
            var tileData = data.GetRepresentation<MapData>();
            var c = new WFCController(MainView, tileData);
            AddController(c);
        }
    }
}