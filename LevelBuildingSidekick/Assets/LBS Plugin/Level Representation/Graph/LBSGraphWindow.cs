using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphWindow : EditorWindow
    {
        LBSGraphView graphView;

        [MenuItem("Level Building Sidekick/Testing")]
        public static void ShowExample()
        {
            LBSGraphWindow wnd = GetWindow<LBSGraphWindow>();
            wnd.titleContent = new GUIContent("GraphWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Assets/GraphWindow.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Assets/GraphWindow.uss");
            root.styleSheets.Add(styleSheet);

            graphView = root.Q<LBSGraphView>();
        }

        private void OnSelectionChange()
        {
            graphView.PopulateView();
        }
    }
}
