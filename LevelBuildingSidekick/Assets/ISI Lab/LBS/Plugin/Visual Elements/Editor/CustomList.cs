using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomList : VisualElement // terminar de implementar para que funcion como "ListView" (!!!)
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CustomList, VisualElement.UxmlTraits> { }
    #endregion

    private List<LBSIdentifierBundle> target;

    public Button plus;
    public Button less;

    public VisualElement header;
    public ListView content;

    public CustomList()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CustomList");
        visualTree.CloneTree(this);

        header = this.Q<VisualElement>("Header");
        content = this.Q<ListView>("Content");

        less = this.Q<Button>("Less");
        less.clicked += DestroyElement;

        plus = this.Q<Button>("Plus");
        plus.clicked += CreateElement;



        content.bindItem += BindItem;
        //content.fixedItemHeight = 160;
        content.makeItem += MakeItem;
        content.itemsChosen += OnItemChosen;
        content.selectionChanged += OnSelectionChange;
        content.style.flexGrow = 1.0f;
    }

    private VisualElement MakeItem()
    {
        var item = new TagBundleView();
        return item;
    }

    private void BindItem(VisualElement element, int index)
    {
        var view = element as TagBundleView;
        var bundle = target[index];
        view.SetInfo(bundle);
    }

    public void SetInfo(List<LBSIdentifierBundle> bundles)
    {
        this.target = bundles; 
        content.bindItem = BindItem;
        content.itemsSource = target;
        content.Rebuild();
    }

    public void CreateElement()
    {
        var settings = LBSSettings.Instance;
        var path = settings.paths.tagFolderPath;

        var name = "/" + "LBSTagsbundles.asset";
        var bundle = ScriptableObject.CreateInstance<LBSIdentifierBundle>();
        AssetDatabase.CreateAsset(bundle, path + name);
        AssetDatabase.SaveAssets();
    }

    public void DestroyElement()
    {
    }

    public void AddElement(VisualElement element)
    {
        content.Add(element);
    }

    public void RemoveElement(VisualElement element)
    {

    }

    public void OnSelectionChange(IEnumerable<object> objs)
    {
        var selected = objs.ToList()[0] as LBSIdentifier;
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }
}
