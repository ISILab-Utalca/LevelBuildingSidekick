using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ISILab.Commons;
using UnityEngine.UIElements;

public class LBSGlobalTagsInspector : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSGlobalTagsInspector, VisualElement.UxmlTraits> { }
    #endregion

    private List<LBSIdentifierBundle> targets;

    private ListView list;
    private Button AddBtn;
    private Button RemoveBtn;

    private LBSIdentifierBundle selected;

    public LBSGlobalTagsInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalTagsInspector");
        visualTree.CloneTree(this);

        this.targets = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>().ToList();

        list = this.Q<ListView>();
        AddBtn = this.Q<Button>("Plus");
        AddBtn.clicked += CreateBundle;
        RemoveBtn = this.Q<Button>("Less");
        RemoveBtn.clicked += DeleteBundle;

        list.itemsSource = targets;
        list.makeItem = MakeItem;
        list.bindItem = BindItem;

        //list.itemsAdded += OnItemAdded;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;
        list.style.flexGrow = 1.0f;

        list.RegisterCallback<GeometryChangedEvent>(evt => UpdateItemHeights(evt));
    }

    private VisualElement MakeItem()
    {
        return new TagBundleView();
    }

    private void BindItem(VisualElement ve, int index)
    {
        if (index >= this.targets.Count())
            return;

        var view = (ve as TagBundleView);
        view.SetInfo(targets[index]);

        if (index == targets.Count - 1)
        {
            view.box.style.borderBottomLeftRadius = 3;
            view.box.style.borderBottomRightRadius = 3;
            view.box.SetBorder(Color.red, 0);
        }
    }

    private void DeleteBundle()
    {
        if (selected == null)
            return;

        var path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        targets = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>().ToList();
        list.itemsSource = targets;

        list.Rebuild();
        //list.RefreshItems();
    }

    private void CreateBundle()
    {
        var nSO = ScriptableObject.CreateInstance<LBSIdentifierBundle>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(targets.Select(b => b.name), "tagBundle");

        AssetDatabase.CreateAsset(nSO, settings.tagFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        targets = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>().ToList();
        list.itemsSource = targets;

        list.Rebuild();
        //list.RefreshItems();
    }

    private void OnItemChosen(IEnumerable<object> objects)
    {
        //Debug.Log(objects);
    }

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = objects.ToList()[0] as LBSIdentifierBundle;
    }

    private void UpdateItemHeights(GeometryChangedEvent evt)
    {

    }
}
