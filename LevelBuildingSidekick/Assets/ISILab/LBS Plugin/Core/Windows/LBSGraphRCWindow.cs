using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LBS.Transformers;
using System;
using UnityEditor.Overlays;
using LBS;
using LBS.Graph;

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


        // este metodo deberia tener parametros tipo (out action, out nextW, out prevW)
        // para boligar a que se immplementen estas coas aqui y no se tenga que intuir. (?)
        public override void OnInitPanel()
        {
            actions.Add(new System.Tuple<string, System.Action>(
                "Generate Schema",
                () => {
                    var c = GetController<LBSGraphRCController>();
                    c.GenerateSchema();
                    var w = GetWindow<LBSSchemaWindow>();
                    w.RefreshView();
                }));

            nextWindow = typeof(LBSSchemaWindow);
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)
            var graphData = data.GetRepresentation<LBSGraphData>();
            AddController(new LBSGraphRCController(MainView, graphData));
        }
    }
}
