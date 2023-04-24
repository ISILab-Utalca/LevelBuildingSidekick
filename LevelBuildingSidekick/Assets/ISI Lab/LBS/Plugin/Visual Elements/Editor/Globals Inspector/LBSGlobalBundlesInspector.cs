using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private LBSIdentifierBundle selected;

    public LBSGlobalBundlesInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
        visualTree.CloneTree(this);

        this.targets = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();

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
    }

    private VisualElement MakeItem()
    {
        return new VisualElement();//new BundleView();
    }

    private void BindItem(VisualElement ve, int index)
    {

    }

    private void OnItemChosen(IEnumerable<object> objects)
    {

    }

    private void OnSelectionChange(IEnumerable<object> objects)
    {
        selected = objects.ToList()[0] as LBSIdentifierBundle;
    }

    private void CreateBundle()
    {

    }

    private void DeleteBundle()
    {

    }
}
