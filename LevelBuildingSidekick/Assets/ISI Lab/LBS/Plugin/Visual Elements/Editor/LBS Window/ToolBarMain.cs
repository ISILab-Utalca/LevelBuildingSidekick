using ISILab.Commons.Utility.Editor;
using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolBarMain : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ToolBarMain, VisualElement.UxmlTraits> { }

    public LBSMainWindow window;

    public event Action<LoadedLevel> OnLoadLevel;
    public event Action<LoadedLevel> OnNewLevel;

    public ToolBarMain()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolBarMain");
        visualTree.CloneTree(this);

        // File menu option
        var fileMenu = this.Q<ToolbarMenu>("ToolBarMenu");
        fileMenu.menu.AppendAction("New", NewLevel);
        fileMenu.menu.AppendAction("Load", LoadLevel);
        fileMenu.menu.AppendAction("Save", SaveLevel);
        fileMenu.menu.AppendAction("Save as", SaveAsLevel);

        var keyMapBtn = this.Q<ToolbarButton>("KeyMapBtn");
        keyMapBtn.clicked += () => { KeyMapWindow.ShowWindow(); };

        // file name label
        var label = this.Q<Label>("IsSavedLabel"); // TODO: mark as unsaved when changes are made
    }

    public void NewLevel(DropdownMenuAction dma)
    {
        var data = LBSController.CreateNewLevel("new file", new Vector3(100, 100, 100));
        OnNewLevel?.Invoke(data);
    }

    public void LoadLevel(DropdownMenuAction dma)
    {
        var data = LBSController.LoadFile();
        if(data != null)
            OnLoadLevel?.Invoke(data);
    }

    public void SaveLevel(DropdownMenuAction dma)
    {
        LBSController.SaveFile();
    }

    public void SaveAsLevel(DropdownMenuAction dma)
    {
        LBSController.SaveFileAs();
    }
}
