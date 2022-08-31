using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public abstract class LBSEditorWindow : EditorWindow
{
    protected VisualElement root;

    public abstract void OnCreateGUI();
    public abstract void OnFocus();

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        OnCreateGUI();

        // root toolbar
        var toolBar = new Toolbar();
        root.Insert(0, toolBar);

        // File menu option
        var fileMenu = new ToolbarMenu();
        fileMenu.text = "File";
        fileMenu.menu.AppendAction("Level.../Load", (dma) => { LBSController.LoadFile(); });
        fileMenu.menu.AppendAction("Level.../Save", (dma) => { LBSController.SaveFile(); });
        fileMenu.menu.AppendAction("Level.../Save as", (dma) => { LBSController.SaveFileAs(); });
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Representation.../Load", (dma) => { Debug.LogError("[Implementar loadRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Representation.../Save", (dma) => { Debug.LogError("[Implementar saveRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Representation.../Save as", (dma) => { Debug.LogError("[Implementar saveRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Help.../Documentation", (dma) => { Debug.LogError("[Implementar documnetation]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Help.../About", (dma) => { Debug.LogError("[Implementar about]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Close", (dma) => { this.Close(); });
        fileMenu.menu.AppendAction("Close All", (dma) => { this.CloseAll(); });
        toolBar.Add(fileMenu);

        // search object in current window
        var search = new ToolbarPopupSearchField();
        search.tooltip = "[Implementar]";
        toolBar.Add(search);

        // file name label
        var label = new Label();
        var fileInfo = LBSController.CurrentLevel.FileInfo;
        if (fileInfo != null)
        {
            label.text = "file: ''" + fileInfo.Name + "''*";
            label.style.color = new Color(0.6f, 0.6f, 0.6f);
        }
        else
        {
            label.text = "file: ''Unsaved''";
            label.style.color = new Color(0.6f, 0.6f, 0.6f);
        }
        toolBar.Add(label);

       
    }

    private void CloseAll()
    {
        var types = Reflection.GetAllSubClassOf<LBSEditorWindow>().ToList();
        types.ForEach((t) => {
            MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.HasOpenInstances)); // magia
            MethodInfo generic = method.MakeGenericMethod(t);
            if ((bool)generic?.Invoke(this, null))
                EditorWindow.GetWindow(t).Close();
        });
    }

    private void OnDestroy()
    {
        /*
        var answer = EditorUtility.DisplayDialog(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "save",
                   "discard");

        if (answer)
            LBSController.SaveFile();

        */
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

}
