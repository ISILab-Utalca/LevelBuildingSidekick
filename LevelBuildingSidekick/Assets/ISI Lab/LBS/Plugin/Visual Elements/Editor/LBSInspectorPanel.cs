using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSInspectorPanel : VisualElement 
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region SINGLETON
    private static LBSInspectorPanel instance;
    public static LBSInspectorPanel Instance
    { 
        get 
        {
            return instance; 
        } 
    }
    #endregion

    #region FIELDS
    private VisualElement content;
    private ButtonGroup tabsGroup;
    private string selectedTab;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    public LBSLocalCurrent current;
    private LBSLocalBehaviours behaviours;
    private LBSLocalAssistants assistants;

    private Dictionary<string, LBSInspector> VEs = new ();
    #endregion

    #region EVENTS
    public event Action<string> OnChangeTab;
    #endregion

    #region CONSTRUCTORS
    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // Tabs
        tabsGroup = this.Q<ButtonGroup>("SubTabs");
        InitTabs();

        // Content
        content = this.Q<VisualElement>("InspectorContent");

        SetSelectedTab("Current data");

        instance = this;
    }
    #endregion

    #region METHODS
    private void InitTabs()
    {
        this.current = new LBSLocalCurrent();
        AddTab("Current data", current);

        this.behaviours = new LBSLocalBehaviours();
        AddTab("Behaviours", behaviours);

        this.assistants = new LBSLocalAssistants();
        AddTab("Assistants", assistants);

        tabsGroup.OnChangeTab += (tab) =>
        {
            this.ClearContent();
            VEs.TryGetValue(tab, out var inspct);
            this.SetContent(inspct);

            OnChangeTab?.Invoke(tab);
        };
    }

    private void AddTab(string tab, LBSInspector element)
    {
        VEs.Add(tab, element);

        tabsGroup.AddChoice(tab, (btn) => 
        {
            var grupableBtn = btn as GrupalbeButton;
            this.ClearContent();
            VEs.TryGetValue(grupableBtn.text, out var inspct);
            this.SetContent(inspct);
        });
    }

    public void SetSelectedTab(string name)
    {
        this.ClearContent();
        VEs.TryGetValue(name, out var ve);
        this.SetContent(ve);

        OnChangeTab?.Invoke(name);
    }

    private void ClearContent()
    {
        content.Clear();
    }

    private void SetContent(VisualElement inspector)
    {
        if (inspector == null)
            return;

        content.Add(inspector);
        (inspector as LBSInspector).Repaint();
    }

    internal void OnSelectedLayerChange(LBSLayer layer)
    {
        foreach (var ve in VEs)
        {
            var inspector = ve.Value;
            inspector.OnLayerChange(layer);
        }
    }
    #endregion

    #region FUNCTIONS SINGLETON
    public static void ShowInspector(string tab)
    {
        var panel = LBSInspectorPanel.Instance;
        panel.VEs.TryGetValue(tab, out var ve);
        panel.tabsGroup.ChangeActive(tab);
    }
    #endregion
}
