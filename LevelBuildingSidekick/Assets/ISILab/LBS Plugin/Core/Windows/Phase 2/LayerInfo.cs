using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayerInfo : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LayerInfo, VisualElement.UxmlTraits> { }

    public VisualElement icnElement;
    public Label nameElement;

    public LayerInfo()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersInfoUXML");
        visualTree.CloneTree(this);

        icnElement = this.Q<VisualElement>(name: "IconElement");
        nameElement = this.Q<Label>(name: "NameElement");

        nameElement.text = "Name Element";
    }

    public LayerInfo(Texture2D texture2D, string labelText)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersInfoUXML");
        visualTree.CloneTree(this);

        icnElement = this.Q<VisualElement>(name: "IconElement");
        nameElement = this.Q<Label>(name: "NameElement");

        icnElement.style.backgroundImage = texture2D;
        nameElement.text = labelText;
    }
}
