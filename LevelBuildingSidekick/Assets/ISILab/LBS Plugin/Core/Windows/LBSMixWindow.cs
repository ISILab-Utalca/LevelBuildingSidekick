using LBS.Representation.TileMap;
using LBS.View;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSMixWindow : LBSEditorWindow
{
    public List<Manipulator> manipulators = new List<Manipulator>(); // ponerlo obligatorio en la herencia (???)

    private VisualElement mainContent;
    private FreeStampView fsv;
    private TileGridView tgv;

    [MenuItem("ISILab/LBS plugin/TEST MIX")]
    [LBSWindow("test mix")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<LBSMixWindow>();
        wnd.titleContent = new GUIContent("TEST MIX");
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("testmix");
        this.ImportStyleSheet("GraphWindow");

        mainContent = root.Q<VisualElement>("MainContent");
        fsv = root.Q<FreeStampView>();
        tgv = root.Q<TileGridView>();
        AddManipulators();
    }

    public void AddManipulators() // poner obligatorio enel padre como herencia (???)
    {
        manipulators.Add(new ContentZoomer());
        manipulators.Add(new ContentDragger());
        manipulators.Add(new SelectionDragger());
        manipulators.Add(new RectangleSelector());

        manipulators.ForEach(m => fsv.AddManipulator(m));
        manipulators.ForEach(m => tgv.AddManipulator(m));
    }

    public override void OnFocus()
    {
        if (fsv == null || tgv == null)
            return;

        var data = LBSController.CurrentLevel.data;
        fsv.Populate(data.GetRepresentation<LBSStampGroupData>());
        tgv.Populate(data.GetRepresentation<LBSTileMapData>());
    }

}
