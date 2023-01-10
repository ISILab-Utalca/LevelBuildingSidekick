using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersPanel : VisualElement
{
    public LayersPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);
    }
}
