using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.InputField;

public class TagBundleView : VisualElement
{
    public LBSIdentifierBundle target;

    public VisualElement box;
    private Toggle toggle;
    private TextField groupNameField;
    private ListView list;
    private Button addBtn;
    private Button removeBtn;

    private LBSIdentifier selected;

    public TagBundleView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagBundleView");
        visualTree.CloneTree(this);

        this.box = this.Q<VisualElement>("Box");
        this.toggle = this.Q<Toggle>();
        this.groupNameField = this.Q<TextField>();

        groupNameField.RegisterCallback<BlurEvent>(e => OnTextChange(groupNameField.value));
        //groupNameField.RegisterCallback<ChangeEvent<string>>(e => OnTextChange(e.newValue));

        list = this.Q<ListView>();

        list.fixedItemHeight = 22;
        list.makeItem += MakeItem;
        list.bindItem += BindItem;
        list.onItemsChosen += OnItemChosen;
        list.onSelectionChange += OnSelectionChange;
        list.style.flexGrow = 1.0f;

        addBtn = this.Q<Button>("Add");
        addBtn.clicked += CreateTag;
        removeBtn = this.Q<Button>("Remove");
        removeBtn.clicked += RemoveTag;
    }

    private VisualElement MakeItem()
    {
        return new TagView();
    }

    private void CreateTag()
    {
        var name = ISILab.Commons.Commons.CheckNameFormat(target.Tags.Select(b => b.name), "new tag");

        var nTag = ScriptableObject.CreateInstance<LBSIdentifier>();
        nTag.Init(name, new Color().RandomColor(), null);

        var settings = LBSSettings.Instance;

        AssetDatabase.CreateAsset(nTag, settings.tagFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        Debug.Log(target.GetInstanceID());
        if(!target.Tags.Contains(nTag))
            target.AddTag(nTag); 

        list.itemsSource = target.Tags;

        list.Rebuild();
        list.RefreshItems();
    }

    private void RemoveTag()
    {
        if (selected == null)
            return;

        target.Remove(selected);

        var path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        
        list.itemsSource = target.Tags;
        list.Rebuild();
    }

    private void OnTextChange(string value)
    {
        target.name = value;
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), target.name);
        EditorUtility.SetDirty(target);
    }


    public void SetInfo(LBSIdentifierBundle tagBundle)
    {
        target = tagBundle;
        list.itemsSource = target.Tags;

        groupNameField.value = target.name;

        list.Rebuild();
    }

    public void BindItem(VisualElement ve, int index)
    {
        var view = (ve as TagView);

        if (index >= this.target.Tags.Count())
            return;

        var tag = this.target.GetTag(index);
        view.SetInfo(tag);
    }


    public void OnSelectionChange(IEnumerable<object> objs)
    {
        selected = objs.ToList()[0] as LBSIdentifier;
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
