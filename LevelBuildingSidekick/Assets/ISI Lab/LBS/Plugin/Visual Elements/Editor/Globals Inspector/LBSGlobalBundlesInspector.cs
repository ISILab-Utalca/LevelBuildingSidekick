using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSGlobalBundlesInspector : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSGlobalBundlesInspector, VisualElement.UxmlTraits> { }
    #endregion

    private List<Tuple<Bundle_Old, int>> targets;

    private ListView list;
    private Button AddBtn;
    private Button RemoveBtn;
    private GeneralBundlesPanel generalPanel;
    private CharacteristicsPanel characteristicsPanel;

    private Bundle_Old selected;

    public LBSGlobalBundlesInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
        visualTree.CloneTree(this);

        //this.targets = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        var all = Utility.DirectoryTools.GetScriptables<Bundle_Old>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle_Old, int>>());

        list = this.Q<ListView>("BundleList");
        list.itemsSource = targets;
        list.makeItem = MakeItem;
        list.bindItem = BindItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;
        list.style.flexGrow = 1.0f;

        AddBtn = this.Q<Button>("Plus");
        AddBtn.clicked += CreateBundle;
        RemoveBtn = this.Q<Button>("Less");
        RemoveBtn.clicked += DeleteBundle;

        this.generalPanel = this.Q<GeneralBundlesPanel>("GeneralPanel");
        this.characteristicsPanel = this.Q<CharacteristicsPanel>("CharacteristicsPanel");

        if (selected == null)
        {
            this.generalPanel.style.display = DisplayStyle.None;
            this.characteristicsPanel.style.display = DisplayStyle.None;
        }
    }

    private List<Tuple<Bundle_Old,int>> OrderList(List<Bundle_Old> bundles, int currentValue, List<Tuple<Bundle_Old, int>> closed)
    {
        var roots = GetRoots(bundles);

        foreach (var root in roots)
        {
            if(closed.Select(t => t.Item1).Contains(root))
            {
                continue;
            }

            closed.Add(new Tuple<Bundle_Old,int>(root, currentValue));

            var bb = root as CompositeBundle;
            if(bb != null && bb.Bundles.Count() > 0)
            {
                var nextValue = currentValue + 1;
                var tempClosed = OrderList(bb.Bundles,nextValue, closed);
            }
        }

        return closed;
    }

    private List<Bundle_Old> GetRoots(List<Bundle_Old> bundles)
    {
        var toR = new List<Bundle_Old>(bundles);

        foreach (var b in bundles)
        {
            var bb = b as CompositeBundle;
            if (bb != null)
            {
                bb.Bundles.ForEach(c => toR.Remove(c));
            }
        }

        return toR;
    }

    private VisualElement MakeItem()
    {
        return new BundleAssetView();
    }

    private void BindItem(VisualElement ve, int index)
    {
        if (index >= this.targets.Count())
            return;

        var view = (ve as BundleAssetView);
        view.SetInfo(targets[index].Item1, targets[index].Item2);
    }

    private void OnItemChosen(IEnumerable<object> objects)
    {

    }

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = (objects.ToList()[0] as Tuple<Bundle_Old, int>).Item1;

        this.generalPanel.style.display = DisplayStyle.Flex;
        this.characteristicsPanel.style.display = DisplayStyle.Flex;
        this.generalPanel.SetInfo(selected);
        this.characteristicsPanel.SetInfo(selected);
    }

    private void CreateBundle()
    {
        var nSO = ScriptableObject.CreateInstance<Bundle_Old>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(targets.Select(b => b.Item1.name), "Asset bundle");

        AssetDatabase.CreateAsset(nSO, settings.bundleFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        var all = Utility.DirectoryTools.GetScriptables<Bundle_Old>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle_Old, int>>());
        list.itemsSource = targets;

        list.Rebuild();
    }

    private void DeleteBundle()
    {
        if (selected == null)
            return;

        var path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        var all = Utility.DirectoryTools.GetScriptables<Bundle_Old>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle_Old, int>>());
        list.itemsSource = targets;

        list.Rebuild();
    }
}
