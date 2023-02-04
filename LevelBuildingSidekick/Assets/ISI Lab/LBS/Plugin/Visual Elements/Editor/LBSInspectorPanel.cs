using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSInspectorPanel : VisualElement 
{
    public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }

    private VisualElement content;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    //private bool _isLocalTabOpen = false;

    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // LocalTab
        var localTab = this.Q<SimpleGrupableButton>("LocalTab");
        //localTab.OnFocusEvent += () => { _isLocalTabOpen = true; };
        //localTab.OnBlurEvent += () => { _isLocalTabOpen = false; };
        localTab.SetName("Local");

        // GlobalTab
        var globalTab = this.Q<SimpleGrupableButton>("GlobalTab");
        globalTab.SetName("Global");

        // AdvancedTab
        var advancedTab = this.Q<SimpleGrupableButton>("AdvancedTab");
        advancedTab.SetName("Advanced");

        var mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();

        // Content
        content = this.Q<VisualElement>("InspectorContent");
    }

    public LBSInspectorPanel(ref MainView mainView, VisualElement content)
    {
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
