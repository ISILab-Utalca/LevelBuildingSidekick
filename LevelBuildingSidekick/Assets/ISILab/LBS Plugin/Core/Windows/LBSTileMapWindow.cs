using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;
using LevelBuildingSidekick;

public class LBSTileMapWindow : LBSEditorWindow
{
    private TileGridView view;
    private Label notSelectedLabel;

    [MenuItem("LBS/Physic step.../Tile map")]
    [LBSWindow("Tile map")]
    public static void OpenWindow()
    {
        var controller = new TileMapController();

        LBSTileMapWindow wnd = GetWindow<LBSTileMapWindow>();
        wnd.titleContent = new GUIContent("Tile Map");

        var view = wnd.rootVisualElement.Q<TileGridView>();
        //view.data = controller;
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("TileMapEditor");
        this.ImportStyleSheet("TileMapEditor");


        view = root.Q<TileGridView>();

        notSelectedLabel = root.Q<Label>("NotSelected");
    }


    private void ActualizeView()
    {
        view.ClearView();
        var tileMap = LBSController.CurrentLevel.data.GetRepresentation<LBSTileMapData>();
        tileMap.Print();
        if (tileMap != null)
        {
            view.Populate(tileMap);
        }
        /*
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
        }*/
    }

    private void OnFocus()
    {
        ActualizeView();
    }

    private void OnLostFocus()
    {
        ActualizeView();
    }

    private void OnSelectionChange()
    {
        ActualizeView();
    }
}