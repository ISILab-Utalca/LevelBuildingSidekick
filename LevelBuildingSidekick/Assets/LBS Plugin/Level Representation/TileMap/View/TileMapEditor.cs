using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;

public class TileMapEditor : EditorWindow
{
    private TileGridView tileGridView;
    private Label notSelectedLabel;
    private InspectorView inspectorView;

    [MenuItem("LBS/TileMap Window...")]
    public static void OpenWindow()
    {
        var controller = new TileMapController();

        TileMapEditor wnd = GetWindow<TileMapEditor>();
        wnd.titleContent = new GUIContent("TileMapEditor");

        var view = wnd.rootVisualElement.Q<TileGridView>();
        view.controller = controller;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileMapEditor");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("TileMapEditor"); 
        root.styleSheets.Add(styleSheet);

        tileGridView = root.Q<TileGridView>();
        notSelectedLabel = root.Q<Label>("NotSelected");
        inspectorView = root.Q<InspectorView>();
    }

    private void OnSelectionChange()
    {
        var c = tileGridView.controller;
        var map = c.Data as LBSTileMapData;
        if(map != null)
        {
            notSelectedLabel.visible = false;
            tileGridView.visible = true;
            tileGridView.SetView(c);
        }
        else
        {
            notSelectedLabel.visible = true;
            tileGridView.visible = false;
        }
    }
}