using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolKit_Layout_Interior : VisualElement
{
    public ToolKit_Layout_Interior()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolKit_Layout_Interior"); // Editor
        visualTree.CloneTree(this);


    }
}
