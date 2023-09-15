using LBS.Bundles;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralBundlesPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<GeneralBundlesPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region FILEDS VIEWS
    // Main content
    private VisualElement mainContent;
    private Foldout mainFoldout;

    // Basic info
    private ObjectField tagField;
    private TextField nameField;

    // Extra info
    private ObjectField fatherField;

    // Assets list
    private ListView assetsList;
    #endregion

    #region FIELDS
    private Bundle target;
    #endregion

    public GeneralBundlesPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GeneralBundlesPanel");
        visualTree.CloneTree(this);

        // Main content
        this.mainContent = this.Q<VisualElement>("MainContent");
        this.mainFoldout = this.Q<Foldout>("MainFoldout");
        this.mainFoldout.RegisterCallback<ChangeEvent<bool>>(e => mainContent.SetDisplay(e.newValue));

        // Basic info
        this.nameField = this.Q<TextField>("NameField");
        nameField.RegisterCallback<BlurEvent>(t => {
            var all = LBSAssetsStorage.Instance.Get<Bundle>();
            var name = ISILab.Commons.Commons.CheckNameFormat(all.Select(b => b.name), nameField.value);
            target.name = name;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), nameField.value);
        });

        this.tagField = this.Q<ObjectField>("TagField");
        tagField.RegisterCallback<ChangeEvent<Object>>(t =>
        {
            //target.ID = t.newValue as LBSIdentifier;
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        });

        // Extra info
        this.fatherField = this.Q<ObjectField>("FatherField");
        fatherField.RegisterCallback<ChangeEvent<Object>>(t => {
            var last = t.previousValue as Bundle;
            if (last != null)
            {
                last.RemoveChild(target);
                EditorUtility.SetDirty(last);
            }

            var current = t.newValue as Bundle;
            if (current != null)
            {
                current.AddChild(target);
                EditorUtility.SetDirty(current);
            }
            AssetDatabase.SaveAssets();
        });

        // Assets list
        this.assetsList = this.Q<ListView>("AssetsList");
        assetsList.makeItem = MakeItem;
        assetsList.bindItem = (view, i) => BindItem(view as ObjectField, i);
        assetsList.style.flexGrow = 1.0f;

        assetsList.Q<Button>("unity-list-view__add-button").clicked += () =>
        {
            target.Assets = assetsList.itemsSource.Cast<Asset>().ToList();
            AssetDatabase.SaveAssets();
        };

        assetsList.Q<Button>("unity-list-view__remove-button").clicked += () =>
        {
            target.Assets = assetsList.itemsSource.Cast<Asset>().ToList();
            AssetDatabase.SaveAssets();
        };
    }

    private VisualElement MakeItem()
    {
        var view = new ObjectField();
        view.objectType = typeof(GameObject);
        view.allowSceneObjects = true;
        return view;
    }

    private void BindItem(VisualElement view, int index)
    {
        var field = (view as ObjectField);

        var item = assetsList.itemsSource[index];
        field.value = item as GameObject;

        field.RegisterValueChangedCallback((value) =>
        {
            assetsList.itemsSource[index] = value.newValue;
            target.Assets = assetsList.itemsSource.Cast<Asset>().ToList();
            AssetDatabase.SaveAssets();
        });

    }

    public void SetInfo(Bundle target)
    {
        this.target = target;

        nameField.SetValueWithoutNotify(target.name);
        //tagField.SetValueWithoutNotify(target.ID);

        fatherField.SetValueWithoutNotify(target.Parent());
     
        assetsList.itemsSource = target.Assets;
    }

    private LBSIdentifier CreateID()
    {
        var all = Utility.DirectoryTools.GetScriptables<LBSIdentifier>().ToList();
        var nSO = ScriptableObject.CreateInstance<LBSIdentifier>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(all.Select(b => b.name), "new ID");

        AssetDatabase.CreateAsset(nSO, settings.paths.tagFolderPath + "/" + name + ".asset");
        // nSO.name = name;
        // AssetDatabase.AddObjectToAsset(nSO, target);
        AssetDatabase.SaveAssets();

        return nSO;
    }
}
