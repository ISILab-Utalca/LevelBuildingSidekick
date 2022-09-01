using LBS.Representation.TileMap;
using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TestMainWindow : LBSEditorWindow
{
    public LBSStampController stampController;
    public LBSTileMapController tileMapController;

    public MainView mainView;

    private TestMainWindow() { }

    [MenuItem("ISILab/LBS plugin/TEST MAIN")]
    [LBSWindow("Main Window")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<TestMainWindow>();
        wnd.titleContent = new GUIContent("test main window");
    }

    public override void OnCreateGUI()
    {
        this.ImportUXML("MainView");
        //this.ImportStyleSheet("GraphWindow");

        var view = rootVisualElement.Q<MainView>();

        var data = LBSController.CurrentLevel.data;
        tileMapController = new LBSTileMapController(data.GetRepresentation<LBSTileMapData>());
        tileMapController.PopulateView(view);
        tileMapController.SetContextualMenu(view);
        stampController = new LBSStampController(data.GetRepresentation<LBSStampGroupData>());
        stampController.PopulateView(view);
        stampController.SetContextualMenu(view);
        
    }

    public override void OnFocus()
    {

    }
}

public class MainView : GraphView
{
    public Action<ContextualMenuPopulateEvent> OnBuild;

    public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }

    public MainView()
    {
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        OnBuild?.Invoke(evt);
        //evt.menu.AppendAction("name", (dma) => { });
    }
}