using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolBarMain : VisualElement // esto puede ser directamente toolbar
{
    public new class UxmlFactory : UxmlFactory<ToolBarMain, VisualElement.UxmlTraits> { }

    public LBSMainWindow window; // (!!) esta referencia solo existe por que actualmente tenemos la data con la que estamos trabajando en mainWindow 

    //public event Action<LoadedLevel> OnChangeLevelData;

    public event Action<LoadedLevel> OnLoadLevel;
    public event Action<LoadedLevel> OnNewLevel;

    public ToolBarMain()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolBarMain"); // Editor
        visualTree.CloneTree(this);

        // File menu option
        var fileMenu = this.Q<ToolbarMenu>("ToolBarMenu");
        fileMenu.menu.AppendAction("New", NewLevel);
        fileMenu.menu.AppendAction("Load", LoadLevel);
        fileMenu.menu.AppendAction("Save", SaveLevel);
        fileMenu.menu.AppendAction("Save as", SaveAsLevel);

        var keyMapBtn = this.Q<ToolbarButton>("KeyMapBtn");
        keyMapBtn.clicked += () => { KeyMapWindow.ShowWindow(); };
        // search object in current window
        //var search = this.Q<ToolbarPopupSearchField>("SearchField"); // (!) Implementar

        // file name label
        var label = this.Q<Label>("IsSavedLabel"); // (!) Implementar
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
        //GenericLBSWindow.RefeshAll(this);
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
