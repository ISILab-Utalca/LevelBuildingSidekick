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

    private ButtonGroup globalSubTab;
    private ButtonGroup localSubTabs;
    private ButtonGroup advancedSubTabs;

    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // G subTab
        globalSubTab = this.Q<ButtonGroup>("GlobalSubTabs");
        globalSubTab.Init();

        // L subTab
        localSubTabs = this.Q<ButtonGroup>("LocalSubTabs");
        localSubTabs.Init();

        // A subTab
        advancedSubTabs = this.Q<ButtonGroup>("AdvancedSubTabs");
        advancedSubTabs.Init();

        // LocalTab
        var localTab = this.Q<GrupalbeButton>("LocalTab");
        localTab.clicked += () =>
        {
            ShowSubSet(localSubTabs);
        };

        // GlobalTab
        var globalTab = this.Q<GrupalbeButton>("GlobalTab");
        globalTab.clicked += () =>
        {
            ShowSubSet(globalSubTab);
        };

        // AdvancedTab
        var advancedTab = this.Q<GrupalbeButton>("AdvancedTab");
        advancedTab.clicked += () =>
        {
            ShowSubSet(advancedSubTabs);
        };

        // MainButtonGroup
        var mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();

        // Content
        content = this.Q<VisualElement>("InspectorContent");

        ShowSubSet(localSubTabs);
    }

    private void ShowSubSet(ButtonGroup target)
    {
        globalSubTab.style.display = (globalSubTab == target) ? DisplayStyle.Flex : DisplayStyle.None;
        localSubTabs.style.display = (localSubTabs == target) ? DisplayStyle.Flex : DisplayStyle.None;
        advancedSubTabs.style.display = (advancedSubTabs == target) ? DisplayStyle.Flex : DisplayStyle.None;
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
