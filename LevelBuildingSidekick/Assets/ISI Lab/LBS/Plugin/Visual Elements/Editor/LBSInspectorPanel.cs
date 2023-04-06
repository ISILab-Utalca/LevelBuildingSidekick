using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSInspectorPanel : VisualElement 
{
    public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }

    private MainView view;
    private VisualElement content;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    private ButtonGroup subTabs;

    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // SubTabs
        subTabs = this.Q<ButtonGroup>("SubTabs");
        subTabs.Init();

        // MainButtonGroup
        var mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();

        // Content
        content = this.Q<VisualElement>("InspectorContent");
    }

    public LBSInspectorPanel(ref MainView mainView, VisualElement content)
    {
        this.view = mainView;
        this.content = content;
    }

    public void AddInspector(LBSInspector inspector)
    {
        inspectors.Add(inspector);
        content.Add(inspector);
    }

    public void RemoveInspector(LBSInspector inspector)
    {
        inspectors.Remove(inspector);
        if(content.Contains(inspector))
            content.Remove(inspector);
    }
}
