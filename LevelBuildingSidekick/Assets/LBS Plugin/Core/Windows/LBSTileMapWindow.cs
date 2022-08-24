using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;

public class LBSTileMapWindow : LBSEditorWindow
{
    private TileGridView tileGridView;
    private Label notSelectedLabel;

    [MenuItem("LBS/Physic step.../Tile map")]
    [LBSWindow("Tile map")]
    public static void OpenWindow()
    {
        var controller = new TileMapController();

        LBSTileMapWindow wnd = GetWindow<LBSTileMapWindow>();
        wnd.titleContent = new GUIContent("TileMapEditor");

        var view = wnd.rootVisualElement.Q<TileGridView>();
        view.controller = controller;
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("TileMapEditor");
        this.ImportStyleSheet("TileMapEditor");


        tileGridView = root.Q<TileGridView>();

        notSelectedLabel = root.Q<Label>("NotSelected");
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