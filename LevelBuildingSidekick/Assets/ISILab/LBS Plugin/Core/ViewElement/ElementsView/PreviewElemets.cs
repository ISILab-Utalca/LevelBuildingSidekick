using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewElemets : Button
{
    public new class UxmlFactory : UxmlFactory<PreviewElemets, VisualElement.UxmlTraits> { }

    public VisualElement image;
    public Label elementSelect;

    public PreviewElemets()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Preview");
        visualTree.CloneTree(this);

        elementSelect = this.Q<Label>(name: "nameSelection");
        image = this.Q<VisualElement>(name: "image");

        elementSelect.text = "Name Element";
    }

    public PreviewElemets(string labelText, Texture2D texture)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Element");
        visualTree.CloneTree(this);

        elementSelect = this.Q<Label>(name: "nameSelection");
        image = this.Q<VisualElement>(name: "image");

        elementSelect.text = labelText;
        image.style.backgroundImage = texture;
    }
}
