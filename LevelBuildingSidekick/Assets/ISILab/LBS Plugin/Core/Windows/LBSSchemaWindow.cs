using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;
using LevelBuildingSidekick;
using UnityEditor.Overlays;

public class LBSSchemaWindow : GenericGraphWindow
{
    private TileGridView view;
    private Label notSelectedLabel;

    [MenuItem("ISILab/LBS plugin/Schema window")]
    [LBSWindow("Schema window")]
    public static void OpenWindow()
    {
        LBSSchemaWindow wnd = GetWindow<LBSSchemaWindow>();
        wnd.titleContent = new GUIContent("Schema window");
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("TileMapEditor");
        this.ImportStyleSheet("TileMapEditor");


        view = root.Q<TileGridView>();

        notSelectedLabel = root.Q<Label>("NotSelected");
    }



    public override void OnFocus()
    {
        //ActualizeView();
    }

    public override void OnLoadControllers()
    {
        //throw new System.NotImplementedException();
    }
}