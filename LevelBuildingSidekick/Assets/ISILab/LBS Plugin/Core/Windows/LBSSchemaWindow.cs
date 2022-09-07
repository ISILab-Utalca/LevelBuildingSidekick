using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;
using LevelBuildingSidekick;
using UnityEditor.Overlays;

namespace LBS.Windows
{
    public class LBSSchemaWindow : GenericGraphWindow, ISupportsOverlays
    {
        private LBSSchemaWindow() { }

        [MenuItem("ISILab/LBS plugin/Schema window")]
        [LBSWindow("Schema window")]
        public static void OpenWindow()
        {
            var  wnd = GetWindow<LBSSchemaWindow>();
            wnd.titleContent = new GUIContent("Schema window");
        }

        public override void OnCreateGUI()
        {
            Debug.Log("aaaa");
        }


        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)
            var tileData = data.GetRepresentation<LBSTileMapData>();
            controllers.Add(new LBSTileMapController(MainView, tileData));
        }
    }
}