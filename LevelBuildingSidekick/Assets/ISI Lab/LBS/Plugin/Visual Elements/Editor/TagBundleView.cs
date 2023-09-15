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
    #region FIELDS
    private LBSIdentifierBundle target;
    public LBSIdentifier selected;
    #endregion

    #region VIEW FIELDS
    private Foldout foldout;
    private VisualElement content;
    private TextField groupNameField;
    private ListView list;
    private Button addBtn;
    private Button removeBtn;
    private EnumField typeDropdown;

    private static VisualTreeAsset visualTree;
    #endregion

    #region EVENTS
    public delegate void TagBundleViewEvent(TagBundleView tbv);
    public TagBundleViewEvent OnSelectionChange;
    public TagBundleViewEvent OnRemoveTag;
    public TagBundleViewEvent OnAddTag;
    #endregion

    #region CONSTRUCTORS
    public TagBundleView()
    {
        visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagBundleView");
        visualTree.CloneTree(this);

        // Main Content
        this.content = this.Q<VisualElement>("Content");
        
        // Main Foldout
        this.foldout = this.Q<Foldout>();
        foldout.RegisterCallback<ChangeEvent<bool>>(e => content.SetDisplay(e.newValue));

        // Bundle Namefield
        this.groupNameField = this.Q<TextField>();
        groupNameField.RegisterCallback<BlurEvent>(e => TextChange(groupNameField.value));

        typeDropdown = this.Q<EnumField>("TypeDropdown");
        typeDropdown.RegisterCallback<ChangeEvent<Enum>>((evt) =>
        {
            target.type = (LBSIdentifierBundle.TagType) evt.newValue;
            AssetDatabase.SaveAssets();
        }); 

        // Tags List
        list = this.Q<ListView>();
        list.fixedItemHeight = 22;
        list.makeItem += MakeItem;
        list.bindItem += BindItem;
        list.itemsChosen += ItemChosen;
        list.selectionChanged += SelectionChange;
        list.style.flexGrow = 1.0f;

        // Add Button
        addBtn = this.Q<Button>("Add");
        addBtn.clicked += CreateTag;

        // Remove Button
        removeBtn = this.Q<Button>("Remove");
        removeBtn.clicked += RemoveTag;
    }
    #endregion

    #region METHODS
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
        AssetDatabase.CreateAsset(nTag, settings.paths.tagFolderPath + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        if(!target.Tags.Contains(nTag))
            target.Add(nTag); 

        list.itemsSource = target.Tags;

        list.Rebuild();
        //list.RefreshItems();
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
        OnRemoveTag?.Invoke(this);
    }

    private void TextChange(string value)
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
        typeDropdown.value = target.type;

        list.Rebuild();
    }

    public void BindItem(VisualElement ve, int index)
    {
        var view = (ve as TagView);

        if (index >= this.target.Tags.Count())
            return;

        var tag = this.target.Tags[index];
        view.SetInfo(tag);
    }

    public void SelectionChange(IEnumerable<object> objs)
    {
        selected = objs.ToList()[0] as LBSIdentifier;
        OnSelectionChange?.Invoke(this);
    }

    public void ItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
    #endregion
}
