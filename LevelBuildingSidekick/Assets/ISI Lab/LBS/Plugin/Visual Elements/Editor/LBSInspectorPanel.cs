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
    private MainView view;
    private VisualElement content;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    private ButtonGroup subTab;
    private ButtonGroup mainTab;

    private LBSLocalCurrent current;
    private LBSLocalBehaviours behaviours;
    private LBSLocalAssistants assistants;

    private Dictionary<string, Dictionary<string, LBSInspector>> VEs = new ();
    #endregion

    #region CONSTRUCTORS
    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // SubTabs
        subTab = this.Q<ButtonGroup>("SubTabs");
        subTab.Init();

        // MainButtonGroup
        mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();

        // Content
        content = this.Q<VisualElement>("InspectorContent");

        InitTabs();
        SetTabs("Global");

        instance = this;
    }
    #endregion

    #region METHODS
    private void InitTabs()
    {
        var gb = new LBSGlobalBundlesInspector(); 
        AddTab("Global", "Bundles", gb);

        var gt = new LBSGlobalTagsInspector();
        AddTab("Global", "Tags", gt);

        var l1 = new LBSLocalCurrent();
        AddTab("Local", "Current data", l1);

        this.behaviours = new LBSLocalBehaviours();
        AddTab("Local", "Behaviours", behaviours);

        this.assistants = new LBSLocalAssistants();
        AddTab("Local", "Assistants", assistants);
    }

    private void AddTab(string mainTabName, string subTabName, LBSInspector element)
    {
        if(VEs.ContainsKey(mainTabName))
        {
            VEs.TryGetValue(mainTabName, out var dic);
            if (dic.ContainsKey(subTabName))
            {
                VEs.TryGetValue(mainTabName, out var visualElement);
            }
            else
            {
                dic.Add(subTabName, element);
            }
        }
        else
        {
            var newDic = new Dictionary<string, LBSInspector>();
            newDic.Add(subTabName, element);
            VEs.Add(mainTabName, newDic);
        }

        var mainTabs = mainTab.Children().ToList();
        var subTabs = subTab.Children().ToList();

        mainTabs.Select(s => s.name).Any();

    }

    private void SetTabs(string name)
    {
        mainTab.Choices = string.Join(",", VEs.Select(pv => pv.Key));

        var mTabs = mainTab.Children().Select(st => st as IGrupable).ToList();
        foreach (var btn in mTabs)
        {
            btn.OnFocusEvent += () =>
            {
                var btn2 = btn as GrupalbeButton;
                SetSubTabs(btn2.text);

                this.ClearContent();
                VEs.TryGetValue(btn2.text, out var ve);
                var xx = ve.Keys.ToList();
                ve.TryGetValue(xx[0], out var inspct);
                this.SetContent(inspct);
            };
        }

        SetSubTabs(name);

        this.ClearContent();
        VEs.TryGetValue(name, out var ve);
        var xx = ve.Keys.ToList();
        ve.TryGetValue(xx[0], out var inspct);
        this.SetContent(inspct);
    }

    private void SetSubTabs(string name)
    {
        VEs.TryGetValue(name, out var ve);
        subTab.Choices = string.Join(",", ve.Select(pv => pv.Key));

        var sTabs = subTab.Children().Select(st => st as IGrupable).ToList();
        for (int i = 0; i < sTabs.Count(); i++)
        {
            var btn = sTabs[i];

            btn.OnFocusEvent += () =>
            {
                var btn2 = btn as GrupalbeButton;
                this.ClearContent();
                ve.TryGetValue(btn2.text,out var inspct);
                this.SetContent(inspct);
            };
        }
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
        ((LBSInspector)inspector).Repaint();
    }

    public void AddInspector(LBSInspector inspector, int index = -1) // relacionado con toolkit (!!!)
    {
        inspectors.Add(inspector);
        if (index == -1)
        {
            content.Add(inspector);
        }
        else
        {
            content.Insert(index, inspector);
        }
       
    }

    public void RemoveInspector(LBSInspector inspector) // relacionado con toolkit (!!!)
    {
        inspectors.Remove(inspector);
        if(content.Contains(inspector))
            content.Remove(inspector);
    }

    internal void OnSelectedLayerChange(LBSLayer layer)
    {
        foreach (var ve in VEs)
        {
            foreach (var ve2 in ve.Value)
            {
                var inspector = ve2.Value;
                inspector.OnLayerChange(layer);
            }
        }
    }
    #endregion

    #region FUNCTIONS SINGLETON
    public static void ShowInspector(string tab, string subTab)
    {
        var panel = LBSInspectorPanel.Instance;
        panel.VEs.TryGetValue(tab, out var ve);
        ve.TryGetValue(subTab, out var inspct);

        var vv = panel.mainTab.Choices.Split(",").ToList();
        panel.mainTab.ChangeActive(vv.FindIndex( v => v == tab));

        var vvv = panel.subTab.Choices.Split(",").ToList();
        panel.subTab.ChangeActive(vvv.FindIndex(v => v == subTab));

        //obj.ClearContent();
        //obj.SetContent(inspct);
    }
    #endregion
}
