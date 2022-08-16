using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphWindow : EditorWindow
    {
        public LBSGraphView graphView;

        [MenuItem("LBS/Graph window...")]
        public static void OpenWindow()
        {
            LBSGraphWindow wnd = GetWindow<LBSGraphWindow>();
            wnd.titleContent = new GUIContent("GraphWindow");
            var graph = wnd.rootVisualElement.Q<LBSGraphView>();
            //Debug.Log(graph.GetHashCode());
            graph.controller = new LBSGraphController(graph);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.

            // Import UXML
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GraphWindow");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("GraphWindow");
            root.styleSheets.Add(styleSheet);
             
            graphView = root.Q<LBSGraphView>();
            //Debug.Log(graphView.GetHashCode()); 
            //Debug.Log(graphView.Controller);
        }

        private void OnSelectionChange()
        {
            graphView.PopulateView();
        }
    }
}
