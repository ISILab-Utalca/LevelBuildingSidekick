using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NameLayer : VisualElement
{
    public new class UxmlFactory : UxmlFactory<NameLayer, VisualElement.UxmlTraits> { }

    public VisualElement icn;
    public Foldout layerDetail;

    public NameLayer()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("NameLayer");
        visualTree.CloneTree(this);

        icn = this.Q<VisualElement>(name: "IconLayer");
        layerDetail = this.Q<Foldout>(name: "LayerDetail");

    }

    public NameLayer(Texture2D texture2D)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("NameLayer");
        visualTree.CloneTree(this);

        icn = this.Q<VisualElement>(name: "IconLayer");
        layerDetail = this.Q<Foldout>(name: "LayerDetail");

        icn.style.backgroundImage = texture2D;
    }
}
