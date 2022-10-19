using LBS.Representation.TileMap;
using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Windows
{
    public class LBSPopulationWindow : GenericGraphWindow, ISupportsOverlays
    {
        private LBSPopulationWindow() { }

        [MenuItem("ISILab/LBS plugin/Population Window")]
        [LBSWindow("Population Window")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSPopulationWindow>();
            wnd.titleContent = new GUIContent("Population window");
        }

        public override void OnInitPanel()
        {
            actions.Add(new System.Tuple<string, System.Action>(
                "Generate 3D",
                () => Debug.Log("[Implementar]")
                ));

            actions.Add(new Tuple<string, Action>(
                "Map Elites", 
                () => {
                    var wnd = GetWindow<MapEliteWindow>();
                    wnd.mainView = this;
                })) ;

            nextWindow = typeof(LBSQuestWindow);
            prevWindow = typeof(LBSSchemaWindow);
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)

            AddController(new LBSTileMapController(MainView, data.GetRepresentation<LBSTileMapData>()));
            var c = new LBSStampTileMapController(MainView, data.GetRepresentation<LBSStampGroupData>());
            AddController(c);

            CurrentController = c;
        }

    }
}
