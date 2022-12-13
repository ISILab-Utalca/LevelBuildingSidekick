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
using LBS.Generator;

namespace LBS.Windows
{
    public class LBSPopulationWindow : GenericGraphWindow, ISupportsOverlays
    {
        private LBSPopulationWindow() { }

        //[MenuItem("ISILab/LBS plugin/Population Window")]
        [LBSWindow("Population Window")] // (!) usar esto en el resto de ventanas y usar mas atributos como metadata
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSPopulationWindow>();
            wnd.titleContent = new GUIContent("Population window");
        }

        public override void OnInitPanel()
        {
            var generator = new PopulationGenerator();
            actions.Add(new System.Tuple<string, System.Action>(
                "Generate 3D",
                () => {
                    var data = LBSController.CurrentLevel.data;
                    generator.Init(data);
                    generator.Generate();
                })) ;

            actions.Add(new Tuple<string, Action>(
                "Map Elites",
                () => {
                    var sch = LBSController.CurrentLevel.data.GetRepresentation<LBSSchemaData>();
                    var x = sch.RecalculateTilePos();
                    var stm = LBSController.CurrentLevel.data.GetRepresentation<LBSStampGroupData>();
                    stm.MoveStamp(x);
                    var wnd = GetWindow<MapEliteWindow>();
                    //wnd.populationWindow = this;
                }));

            actions.Add(new Tuple<string, Action>(
                "Brush window",
                () =>
                {
                    var brsh = GetWindow<BrushWindow>();
                    brsh.titleContent = new GUIContent(brsh.GetName());
                }));

            nextWindow = typeof(QuestWindow);
            prevWindow = typeof(LBSSchemaWindow);
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)

            AddController(new LBSTileMapController(MainView, data.GetRepresentation<LBSSchemaData>()));
            var c = new LBSStampTileMapController(MainView, data.GetRepresentation<LBSStampGroupData>());
            AddController(c);

            CurrentController = c;
        }

    }
}

