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

    public class WFCWindow : GenericLBSWindow, ISupportsOverlays
    {
        private WFCWindow() { }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("ISILab/LBS plugin/WFC Window", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<WFCWindow>();
            window.titleContent = new GUIContent("WFC Window");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInitPanel()
        {
            var generator = new WFCGenerator();

            actions.Add(new System.Tuple<string, System.Action>(
                 "Generate 3D",
                 () => {
                     var data = LBSController.CurrentLevel.data;
                     generator.Init(data);
                     generator.Generate();
                 }
                 ));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data;
            var tileData = data.GetRepresentation<MapData>();
            var c = new WFCController(MainView, tileData);
            AddController(c);
        }
    }
}