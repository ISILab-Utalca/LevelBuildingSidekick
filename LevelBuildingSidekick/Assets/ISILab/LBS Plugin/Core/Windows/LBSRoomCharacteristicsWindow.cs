using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LBS.Transformers;

namespace LevelBuildingSidekick.Graph
{
    public class LBSRoomCharacteristicsWindow : LBSEditorWindow
    {
        private LBSGraphView graphView;

        [MenuItem("ISILab/LBS plugin/Physic step.../Room characteristics graph")]
        [LBSWindow("Room Characteristics")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSRoomCharacteristicsWindow>();
            wnd.titleContent = new GUIContent("Room characteristics window");
            Debug.Log(wnd); // son vitales...
            wnd.position = wnd.position; // VITALES!!!!
        }

        public override void OnCreateGUI()
        {
            this.ImportUXML("GraphWindow");
            this.ImportStyleSheet("GraphWindow");

            graphView = root.Q<LBSGraphView>();
            var generateBtn = root.Q<Button>("GenerateBtn");
            generateBtn.clicked += () => { GenerateSchema(); };
        }

        
        private void GenerateSchema()
        {
            Debug.Log("[Generate Tile map]");
            var g = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            var tm = new GraphToTileMap().Transform(g);
            LBSController.CurrentLevel.data.AddRepresentation(tm);
            LBSController.SaveFile(); // esto es necesario?, supongo que si pero no se
        }
        
        private void OnFocus()
        {
            if (graphView != null)
            {
                var data = LBSController.CurrentLevel.data;
                graphView.Populate(data.GetRepresentation<LBSGraphData>());
            }
        }
 
    }
}
