using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TypeBrush : Button
{
    public new class UxmlFactory : UxmlFactory<TypeBrush, VisualElement.UxmlTraits> { }

    public VisualElement contCircle;
    public VisualElement image;

    public TypeBrush()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TypeBrushUXML");
        visualTree.CloneTree(this);

        contCircle = this.Q<VisualElement>(name: "TypeBrush");
        image = this.Q<VisualElement>(name: "Image");

    }

    public TypeBrush(Color color, Texture2D texture)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TypeBrushUXML");
        visualTree.CloneTree(this);

        contCircle = this.Q<VisualElement>(name: "TypeBrush");
        image = this.Q<VisualElement>(name: "Image");

        contCircle.style.backgroundColor = color;
        image.style.backgroundImage = texture;
    }
}
