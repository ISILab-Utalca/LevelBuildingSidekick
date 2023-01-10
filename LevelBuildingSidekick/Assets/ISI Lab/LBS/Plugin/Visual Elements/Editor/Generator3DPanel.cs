using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Generator3DPanel : VisualElement
{
    public Generator3DPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);
    }
}
