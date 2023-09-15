using LBS.Behaviours;
using LBS.Bundles;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSGlobalBundlesInspector : LBSInspector
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSGlobalBundlesInspector, VisualElement.UxmlTraits> { }
    #endregion

    #region FIELDS VIEWS
    // bundles panel
    private ListView list;
    private DropdownField typeField;
    private Button addRoot;
    private Button addChild;
    private Button removeBtn;

    // General panel
    private GeneralBundlesPanel generalPanel;

    // Characteristic panel
    private CharacteristicsPanel characteristicsPanel;
    #endregion

    #region FIELDS
    // Internal
    private Bundle pressetSelected;
    private Bundle selected;
    private List<Tuple<Bundle, int>> targets;
    #endregion

    #region CONSTRUCTORS
    public LBSGlobalBundlesInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
        visualTree.CloneTree(this);

        // Bundle list
        var allBUndles = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        var presetsBundles = allBUndles.Where(b => b.IsPresset && b.IsRoot()).ToList();
        var bundles = allBUndles.Where( b => !b.IsPresset).ToList();

        this.targets = OrderList(bundles, 0, new List<Tuple<Bundle, int>>());

        list = this.Q<ListView>("BundleList");
        list.itemsSource = targets;
        list.makeItem = MakeItem;
        list.bindItem = BindItem;
        list.selectionChanged += OnSelectionChange;
        list.style.flexGrow = 1.0f;

        // select type to add
        typeField = this.Q<DropdownField>("TypeToAdd");
        typeField.choices = presetsBundles.Select( b => b.name).ToList();
        typeField.RegisterCallback<ChangeEvent<string>>(e => {
            pressetSelected = presetsBundles.Find(b => b.name.Equals(e.newValue));
        });
        typeField.value = presetsBundles[0].name;
        pressetSelected = presetsBundles[0];

        // Add button
        addRoot = this.Q<Button>("AddBrother");
        addRoot.clicked += () => CreateBundle(null);//selected?.Parent());
        addChild = this.Q<Button>("AddChild");
        addChild.clicked += () => CreateBundle(selected);

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
    #endregion

    #region METHODS
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
            if (b == null)
                continue;

            if (b.IsLeaf)
            {
                b.ChildsBundles.ForEach(c => toR.Remove(c));
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

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = (objects.ToList()[0] as Tuple<Bundle, int>).Item1;

        this.generalPanel.style.display = DisplayStyle.Flex;
        this.characteristicsPanel.style.display = DisplayStyle.Flex;
        this.generalPanel.SetInfo(selected);
        this.characteristicsPanel.SetInfo(selected);

        Selection.SetActiveObjectWithContext(selected, selected);
    }

    private void CreateBundle(Bundle parent)
    {
        var settings = LBSSettings.Instance;
        var storage = LBSAssetsStorage.Instance;

        var clone = pressetSelected.Clone() as Bundle;
        var name = ISILab.Commons.Commons.CheckNameFormat(targets.Select(b => b.Item1.name), pressetSelected.name);
        
        AssetDatabase.CreateAsset(clone, settings.paths.bundleFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        var all = storage.Get<Bundle>().Where(b => !b.IsPresset).ToList();//Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
        list.itemsSource = targets;

        list.Rebuild();
    }

    private void DeleteBundle()
    {
        if (selected == null)
            return;

        var storage = LBSAssetsStorage.Instance;

        var path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        var all = storage.Get<Bundle>().Where(b => !b.IsPresset).ToList();
        this.targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
        list.itemsSource = targets;

        list.Rebuild();
    }

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        throw new NotImplementedException();
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        //Debug.Log("Actualizacion de layer Global/Bundles inspector");
    }
    #endregion
}
