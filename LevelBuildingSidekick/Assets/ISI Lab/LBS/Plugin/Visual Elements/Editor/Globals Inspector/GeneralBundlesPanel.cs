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

    // Main content
    private VisualElement mainContent;
    private Foldout mainFoldout;

    // Basic info
    private ObjectField iconField;
    private TextField nameField;
    private ColorField colorField;

    // Extra info
    private ListView childList;
    private ObjectField fatherField;

    // Assets list
    private ListView assetsList;

    // Target
    private Bundle target;

    public GeneralBundlesPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GeneralBundlesPanel");
        visualTree.CloneTree(this);

        // Main content
        this.mainContent = this.Q<VisualElement>("MainContent");
        this.mainFoldout = this.Q<Foldout>("MainFoldout");
        this.mainFoldout.RegisterCallback<ChangeEvent<bool>>( e => mainContent.SetDisplay(e.newValue));

        // Basic info
        this.iconField = this.Q<ObjectField>("IconField");
        iconField.RegisterCallback<ChangeEvent<Object>>( t => target.ID.Icon = t.newValue as Texture2D);

        this.nameField = this.Q<TextField>("NameField");
        nameField.RegisterCallback<ChangeEvent<string>>(t => target.ID.Label = target.ID.name = t.newValue);

        this.colorField = this.Q<ColorField>("ColorField");
        colorField.RegisterCallback<ChangeEvent<Color>>(t => target.ID.Color = t.newValue);

        // Extra info
        this.childList = this.Q<ListView>("ChildsList");
        InitChildList();
        //childList.RegisterCallback<ChangeEvent<object>>(e => target.Father() = e.newValue);
        this.fatherField = this.Q<ObjectField>("FatherField");

        // Assets list
        this.assetsList = this.Q<ListView>("AssetsList");
        //InitAssetsList();

       
    }

    private void InitChildList()
    {
        // IMPLEMENTAR (!!!)
    }

    private void InitAssetsList()
    {
        assetsList.makeItem += () =>
        {
            return new ObjectField();
        };

        assetsList.bindItem = (item, index) =>
        {
            var list = (target as SimpleBundle).Assets;
            if (index >= list.Count)
                (target as SimpleBundle).Add(null);

            var view = (item as ObjectField);
            var t = list[index];
            view.value = t;
        };

        /*
        assetsList.Q<Button>("unity-list-view__add-button").clickable = new Clickable(() =>
        {
            Debug.Log("AA");
        });
        */
    }

    public void SetInfo(Bundle target)
    {
        this.target = target;

        if(target.ID == null)
        {
            target.ID = CreateID();
        }

        iconField.value = target.ID.Icon;
        nameField.value = target.ID.Label;
        colorField.value = target.ID.Color;
     
        assetsList.itemsSource = (target as SimpleBundle).Assets;
    }

    private LBSIdentifier CreateID()
    {
        var all = Utility.DirectoryTools.GetScriptables<LBSIdentifier>().ToList();
        var nSO = ScriptableObject.CreateInstance<LBSIdentifier>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(all.Select(b => b.name), "new ID");

        AssetDatabase.CreateAsset(nSO, settings.tagFolderPath + "/" + name + ".asset");
        // nSO.name = name;
        // AssetDatabase.AddObjectToAsset(nSO, target);
        AssetDatabase.SaveAssets();

        return nSO;
    }
}
