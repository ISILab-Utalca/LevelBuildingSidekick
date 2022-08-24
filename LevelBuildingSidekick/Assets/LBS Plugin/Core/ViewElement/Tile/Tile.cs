using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Tile, VisualElement.UxmlTraits> { }

    public Tile()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Tile");
        visualTree.CloneTree(this);

    }

}
