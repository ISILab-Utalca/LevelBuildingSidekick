using LBS.Graph;
using LBS.Manipulators;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Overlays
{
    //[Overlay(typeof(LBSGraphRCWindow), ID, "Tools", "GraphTool", defaultDisplay = true)]
    public class GraphTool : Overlay
    {
        private const string ID = "GraphOverlayTools";

        private LBSGraphRCWindow window;

        private Button selectButton;
        private Button eraseButton;
        private Button zoomButton;
        private Button addNodeButton;
        private Button addEdgeButton;

        public override VisualElement CreatePanelContent() // (?) dudosa la implementacion de alguno de los botones
        {
            var root = new VisualElement();
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GraphTool");
            visualTree.CloneTree(root);

            window = EditorWindow.GetWindow<LBSGraphRCWindow>();

            // Select button
            selectButton = root.Q<Button>("SelectButton");
            selectButton.clicked += () => window.MainView.SetManipulator(new RectangleSelector());

            // Erase button
            eraseButton = root.Q<Button>("EraseButton");
            eraseButton.clicked += () => 
            {
                var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                var c = wnd.GetController<LBSGraphRCController>();
                wnd.MainView.SetManipulator(new DeleteNodeManipulator(wnd, c));
            };

            // Zoom button
            zoomButton = root.Q<Button>("ZoomButton");
            zoomButton.clicked += () => window.MainView.SetManipulator(new ContentZoomer());

            // Add node button
            addNodeButton = root.Q<Button>("AddNodeButton");
            addNodeButton.clicked += () => {
                var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                var c = wnd.GetController<LBSGraphRCController>();
                wnd.MainView.SetManipulator(new AddNodeManipulator(wnd, c));
            };

            // Add edge button
            addEdgeButton = root.Q<Button>("AddEdgeButton");
            addEdgeButton.clicked += () =>
            {
                var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                var c = wnd.GetController<LBSGraphRCController>();
                wnd.MainView.SetManipulator(new ConnectionManipulator(wnd, c));
            };

            return root;
        }
    }
}