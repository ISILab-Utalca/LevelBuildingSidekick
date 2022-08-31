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

    [MenuItem("ISILab/LBS plugin/Physic step.../Tile map")]
    [LBSWindow("Tile map")]
    public static void OpenWindow()
    {
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
        if (view == null)
            return;

        view.ClearView();

        var tileMap = LBSController.CurrentLevel.data.GetRepresentation<LBSTileMapData>();
        if (tileMap != null)
        {
            view.Populate(tileMap);
        }
    }

    public override void OnFocus()
    {
        ActualizeView();
    }

    private void OnLostFocus()
    {
        
        //ActualizeView();
    }

    private void OnSelectionChange()
    {
        //ActualizeView();
    }
}