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

    // bundles panel
    private ListView list;
    private Button addBrother;
    private Button addChild;
    private Button removeBtn;
    private DropdownField typeField;

    // General panel
    private GeneralBundlesPanel generalPanel;

    // Characteristic panel
    private CharacteristicsPanel characteristicsPanel;

    // Internal
    private Bundle selected;
    private List<Tuple<Bundle, int>> targets;

    public LBSGlobalBundlesInspector()
    {
        

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
        visualTree.CloneTree(this);

        // Bundle list
        var all = Utility.DirectoryTools.GetScriptables<Bundle>().Where( b => !b.isPreset).ToList();

        this.targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());

        list = this.Q<ListView>("BundleList");
        list.itemsSource = targets;
        list.makeItem = MakeItem;
        list.bindItem = BindItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;
        list.style.flexGrow = 1.0f;

        // select type to add
        typeField = this.Q<DropdownField>("TypeToAdd");
        //typeField.choices = 

        // Add button
        addBrother = this.Q<Button>("AddBrother");
        addBrother.clicked += CreateBundle;
        addChild = this.Q<Button>("AddChild");
        addBrother.clicked += CreateBundle;

        // remove button
        removeBtn = this.Q<Button>("RemoveBtn");
        removeBtn.clicked += DeleteBundle;

        this.generalPanel = this.Q<GeneralBundlesPanel>("GeneralPanel");
        this.characteristicsPanel = this.Q<CharacteristicsPanel>("CharacteristicsPanel");

        if (selected == null)
        {
            this.generalPanel.style.display = DisplayStyle.None;
            this.characteristicsPanel.style.display = DisplayStyle.None;
        }
    }

    private List<Tuple<Bundle, int>> OrderList(List<Bundle> bundles, int currentValue, List<Tuple<Bundle, int>> closed)
    {
        var roots = GetRoots(bundles);

        foreach (var root in roots)
        {
            if (root == null)
                continue;

            if(closed.Select(t => t.Item1).Contains(root))
            {
                continue;
            }

            closed.Add(new Tuple<Bundle, int>(root, currentValue));

            if(root.ChildsBundles.Count() > 0)
            {
                var nextValue = currentValue + 1;
                var tempClosed = OrderList(root.ChildsBundles, nextValue, closed);
            }
        }

        return closed;
    }

    private List<Bundle> GetRoots(List<Bundle> bundles)
    {
        var toR = new List<Bundle>(bundles);

        foreach (var b in bundles)
        {
            if (b.IsLeaf)
            {
                b.ChildsBundles.ForEach(c => toR.Remove(c));
            }

            /*
            var bb = b as CompositeBundle;
            if (bb != null)
            {
                bb.Bundles.ForEach(c => toR.Remove(c));
            }
            */
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
        selected = (objects.ToList()[0] as Tuple<Bundle, int>).Item1;

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

        var all = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
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

        var all = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
        list.itemsSource = targets;

        list.Rebuild();
    }
}
