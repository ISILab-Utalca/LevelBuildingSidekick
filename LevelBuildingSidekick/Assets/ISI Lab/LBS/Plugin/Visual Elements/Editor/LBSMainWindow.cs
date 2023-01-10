using LBS.ElementView;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSMainWindow : EditorWindow
{

    [MenuItem("ISILab/LBS plugin/Main window", priority = 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = null; // (!!) implementar 
        window.titleContent = new GUIContent("Level builder",icon);
    }

    public virtual void CreateGUI()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");

        // MainView 
        var mainView = rootVisualElement.Q<MainView>("MainView");

        // ToolPanel
        var toolPanel = rootVisualElement.Q<VisualElement>("ToolPanel");

        // InspectorContent
        var inspector = rootVisualElement.Q<VisualElement>("InspectorContent");

        // ExtraPanel
        var extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            extraPanel.Clear();
            extraPanel.Add(new LayersPanel());
        };

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("IAButton");
        IABtn.clicked += () =>
        {
            extraPanel.Clear();
            extraPanel.Add(new AIPanel());
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            extraPanel.Clear();
            extraPanel.Add(new Generator3DPanel());
        };
    }
}
