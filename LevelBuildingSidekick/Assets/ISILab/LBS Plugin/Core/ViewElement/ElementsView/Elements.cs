using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Elements : Button
{
    public new class UxmlFactory : UxmlFactory<Elements, VisualElement.UxmlTraits> { }

    public VisualElement image;
    public Label nameElement;

    public Elements()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Element");
        visualTree.CloneTree(this);

        nameElement = this.Q<Label>(name: "NameE");
        image = this.Q<VisualElement>(name: "image");

        nameElement.text = "Name Element";        
    }

    public Elements(string labelText, Texture2D texture)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Element");
        visualTree.CloneTree(this);

        nameElement = this.Q<Label>(name: "NameE");
        image = this.Q<VisualElement>(name: "image");

        nameElement.text = labelText;
        image.style.backgroundImage = texture;
    }
}
