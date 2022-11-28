using LBS.Manipulators;
using LBS.Representation;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(WFCWindow), ID, "WFCTools", "-", defaultDisplay = true)]
public class WFCTools : Overlay
{
    private const string ID = "WFCToolsOverlay";

    private Button allMode;

    private Button addTileButton;
    private Button eraseTileButton;
    private Button addConectionButton;
    private Button eraseConectionButton;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("WFCTools");
        visualTree.CloneTree(root);

        var window = EditorWindow.GetWindow<WFCWindow>();
        var controller = window.GetController<WFCController>();
        var data = controller.GetData() as LBSTileMapData;

        // All mode 
        allMode = root.Q<Button>("AllMode");  // (!) esto terminara desapareciendo
        allMode.clicked += () => window.MainView.SetBasicManipulators();

        // Add tile
        addTileButton = root.Q<Button>("AddTileButton");
        addTileButton.clicked += () =>
        {
            var manipulator = new AddTileManipulator(data, controller);
            manipulator.OnEndAction += () => { window.RefreshView();};
            window.MainView.SetManipulator(manipulator);
        };

        // Erase tile
        eraseTileButton = root.Q<Button>("EraseTileButton");
        eraseTileButton.clicked += () =>
        {
           var manipulator2 = new DeleteTileManipulator(data, controller);
           manipulator2.OnEndAction += () => { window.RefreshView(); };
           window.MainView.SetManipulator(manipulator2);
        };

        // Add conection
        addConectionButton = root.Q<Button>("AddConectionButton");
        addConectionButton.clicked += () =>
        {
            var manipulator = new AddConnectionManipulator(data, controller);
            manipulator.OnEndAction += () => { window.RefreshView(); };
            window.MainView.SetManipulator(manipulator);
        };

        // Erase connection
        eraseConectionButton = root.Q<Button>("EraseConectionButton");
        eraseConectionButton.clicked += () =>
        {

        };


        return root;
    }
}
