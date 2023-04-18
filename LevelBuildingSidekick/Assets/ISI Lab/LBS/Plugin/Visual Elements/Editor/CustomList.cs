using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomList : ListView // terminar de implementar para que funcion como "ListView" (!!!)
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CustomList, VisualElement.UxmlTraits> { }
    #endregion

    public Button plus;
    public Button less;

    public VisualElement header;
    public VisualElement content;

    public CustomList()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CustomList");
        visualTree.CloneTree(this);

        header = this.Q<VisualElement>("Header");
        content = this.Q<VisualElement>("Content");

        less = this.Q<Button>("Less");
        less.clicked += DestroyElement;

        plus = this.Q<Button>("Plus");
        plus.clicked += CreateElement;
    }

    public void CreateElement()
    {

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
}
