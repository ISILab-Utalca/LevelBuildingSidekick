using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphWindow : LBSWindowEditor
    {
        public LBSGraphView graphView;

        [MenuItem("LBS/Graph window...")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<LBSGraphWindow>();
            wnd.titleContent = new GUIContent("Graph window");
           
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
