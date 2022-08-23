using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSEditorWindow : EditorWindow
{
    protected VisualElement root;

    public abstract void OnCreateGUI();

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        OnCreateGUI();

        var toolBar = new Toolbar();
        var fileMenu = new ToolbarMenu();
        fileMenu.text = "File";
        fileMenu.menu.AppendAction("Save", SaveAction);
        fileMenu.menu.AppendAction("Save as", SaveAsAction);

        root.Insert(0,toolBar);
        toolBar.Add(fileMenu);

        //var toolbar = root.Q<ToolbarMenu>("GeneralToolbar");
        //toolbar.menu.AppendAction("Save", SaveAction);
        //toolbar.menu.AppendAction("Save as", SaveAsAction);


        //var saveBtn = root.Q<ToolbarButton>("SaveButton");
        //saveBtn.clicked += SaveAction;

        //var saveAsBtn = root.Q<ToolbarButton>("SaveAsButton");
        //saveAsBtn.clicked += SaveAsAction;


    }

    protected void ImportUXML(string name)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>(name);
        visualTree.CloneTree(root);
    }

    protected void ImportStyleSheet(string name)
    {
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>(name);
        root.styleSheets.Add(styleSheet);
    }


    private void SaveAction(DropdownMenuAction dma)
    {
        SaveAction();
    }

    private void SaveAsAction(DropdownMenuAction dma)
    {
        SaveAsAction();
    }

    private void SaveAction()
    {
        LBSController.SaveFile();
    }

    private void SaveAsAction()
    {
        LBSController.SaveFileAs();
    }


}
