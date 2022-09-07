using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public abstract class GenericGraphWindow : EditorWindow, ISupportsOverlays
{
    protected List<IRepController> controllers = new List<IRepController>();
    protected IRepController currentController;
    protected VisualElement root;
    protected MainView mainView;

    private Label label;

    public MainView MainView => mainView;

    public abstract void OnCreateGUI();
    public abstract void OnFocus();
    public abstract void OnLoadControllers();

    public void CreateGUI()
    {
        root = rootVisualElement;
        this.ImportUXML("GenericGraphWindowUXML");
        mainView  = rootVisualElement.Q<MainView>();
        InitToolBar();

        //RefreshView();
        controllers.Clear();
        OnLoadControllers();
        InitContextualMenu();
        Populate();

        currentController = controllers[0];
        mainView.OnClearSelection += () =>
        {
             var il = Reflection.MakeGenericScriptable(currentController.GetData()); // temporal (!!!)
             Selection.SetActiveObjectWithContext(il, il);
        };
    }

    public void SwithController(int value)
    {
        if(value < 0 || value >= controllers.Count)
        {
            Debug.LogWarning("Index <b>'"+value+"'</b> is out of bounds.");
        }

    }

    public void RefreshView()
    {
        mainView.graphElements.ForEach(e => mainView.RemoveElement(e));
        
        controllers.Clear();

        OnLoadControllers();
        InitContextualMenu();
        Populate();
    }

    public override void SaveChanges()
    {
        Debug.Log("No se como funciona esta funcion (SaveChanges)");
        base.SaveChanges();
    }

    public override void DiscardChanges()
    {
        Debug.Log("No se como funciona esta funcion (DiscardChanges)");
        base.DiscardChanges();
    }

    public  T GetController<T>()
    {
        return (T)controllers.Find(c => c is T);
    }

    private void Populate()
    {
        controllers.ForEach(c => c.PopulateView(mainView));
    }

    private void InitContextualMenu()
    {
        controllers.ForEach(c => c.SetContextualMenu(mainView));
    }

    private void OnInspectorUpdate()
    {
        var fileInfo = LBSController.CurrentLevel.FileInfo;
        if (fileInfo != null)
        {
            label.text = "file*: ''" + fileInfo.Name + "''";
            label.style.color = new Color(0.6f, 0.6f, 0.6f);
        }
        else
        {
            label.text = "file: ''Unsaved''";
            label.style.color = Color.white;
        }

    }

    private void InitToolBar()
    {
        // root toolbar
        var toolBar = new Toolbar();
        root.Insert(0, toolBar);

        // File menu option
        var fileMenu = new ToolbarMenu();
        fileMenu.text = "File";
        fileMenu.menu.AppendAction("Load", (dma) => {  LBSController.LoadFile(); RefreshView(); });
        fileMenu.menu.AppendAction("Save", (dma) => { LBSController.SaveFile(); });
        fileMenu.menu.AppendAction("Save as", (dma) => { LBSController.SaveFileAs(); });
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
        label = new Label();
        toolBar.Add(label);
    }



    private void CloseAll()
    {
        var types = Reflection.GetAllSubClassOf<GenericGraphWindow>().ToList();
        types.ForEach((t) => {
            MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.HasOpenInstances)); // magia
            MethodInfo generic = method.MakeGenericMethod(t);
            if ((bool)generic?.Invoke(this, null))
                EditorWindow.GetWindow(t).Close();
        });
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

public class MainView : LBSGraphView
{
    public Action<ContextualMenuPopulateEvent> OnBuild;

    //manipulators
    internal ContentZoomer zoomer = new ContentZoomer();
    internal ContentDragger dragger = new ContentDragger();
    internal SelectionDragger selectionDragger = new SelectionDragger();
    internal RectangleSelector rectagleSelector = new RectangleSelector();

    private List<Manipulator> manipulators = new List<Manipulator>();

    public Action OnClearSelection;

    public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }

    public MainView()
    {
        SetBasicManipulators();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        OnBuild?.Invoke(evt);
    }

    public void SetBasicManipulators()
    {
        var manis = new List<Manipulator>() { zoomer, dragger, selectionDragger, rectagleSelector };
        SetManipulators(manis);
    }

    public void SetManipulator(Manipulator current)
    {
        RemoveManipulators(manipulators);
        this.AddManipulator(current);
    }

    public void SetManipulators(List<Manipulator> manipulators)
    {
        ClearManipulators();
        AddManipulators(manipulators);
    }

    public override void ClearSelection()
    {
        base.ClearSelection();
        if (selection.Count == 0)
        {
            OnClearSelection?.Invoke();
            //LBSController.ShowLevelInspector();
        }
    }

    public void ClearManipulators()
    {
        foreach (var m in this.manipulators)
        {
            this.RemoveManipulator(m);
        }
        this.manipulators.Clear();
    }

    public void RemoveManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            this.manipulators.Remove(m);
            this.RemoveManipulator(m);
        }
    }

    public void AddManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            if (!this.manipulators.Contains(m))
            {
                this.manipulators.Add(m);
                this.AddManipulator(m);
            }
        }
    }
}