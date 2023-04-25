using LBS.Settings;
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

    private List<Bundle> targets;

    private ListView list;
    private Button AddBtn;
    private Button RemoveBtn;
    private GeneralBundlesPanel generalPanel;
    private CharacteristicsPanel characteristicsPanel;

    private Bundle selected;

    public LBSGlobalBundlesInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
        visualTree.CloneTree(this);

        this.targets = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        //var all = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        //this.targets = GetRoots(all);

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

        if(selected == null)
        {
            this.generalPanel.style.display = DisplayStyle.None;
            this.characteristicsPanel.style.display = DisplayStyle.None;
        }
    }

    private List<Bundle> GetRoots(List<Bundle> bundles)
    {
        var toR = new List<Bundle>(bundles);

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
        view.SetInfo(targets[index]);
    }

    private void OnItemChosen(IEnumerable<object> objects)
    {

    }

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = objects.ToList()[0] as Bundle;

        this.generalPanel.style.display = DisplayStyle.Flex;
        this.characteristicsPanel.style.display = DisplayStyle.Flex;
        this.generalPanel.SetInfo(selected);
        this.characteristicsPanel.SetInfo(selected);
    }

    private void CreateBundle()
    {
        var nSO = ScriptableObject.CreateInstance<Bundle>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(targets.Select(b => b.name), "Asset bundle");

        AssetDatabase.CreateAsset(nSO, settings.bundleFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        targets = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
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

        targets = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        list.itemsSource = targets;

        list.Rebuild();
    }
}
