using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPanel : VisualElement
{
    public AIPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);
    }
}
