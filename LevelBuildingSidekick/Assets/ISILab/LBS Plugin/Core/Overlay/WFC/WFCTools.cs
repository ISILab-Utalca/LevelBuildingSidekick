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

        // All mode 
        allMode = root.Q<Button>("AllMode");  // (!) esto terminara desapareciendo
        allMode.clicked += () => window.MainView.SetBasicManipulators();

        // Add tile
        addTileButton = root.Q<Button>("AddTileButton");
        addTileButton.clicked += () =>
        {

        };

        // Erase tile
        eraseTileButton = root.Q<Button>("EraseTileButton");
        addTileButton.clicked += () =>
        {

        };

        // Add conection
        addConectionButton = root.Q<Button>("AddConectionButton");
        addTileButton.clicked += () =>
        {

        };

        // Erase connection
        eraseConectionButton = root.Q<Button>("EraseConectionButton");
        addTileButton.clicked += () =>
        {

        };


        return root;
    }
}
