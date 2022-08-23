using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LevelBuildingSidekick.Graph
{
    public class LBSRoomCharacteristicsWindow : LBSEditorWindow
    {
        public LBSGraphView graphView;

        [MenuItem("LBS/Physic step.../Room characteristics graph")]
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
        }

        private void OnGUI()
        {
            //graphView.OnGUI();
        }

        private void OnFocus()
        {
            if (graphView != null)
            {
                graphView.PopulateView();
            }
        }

    }
}
