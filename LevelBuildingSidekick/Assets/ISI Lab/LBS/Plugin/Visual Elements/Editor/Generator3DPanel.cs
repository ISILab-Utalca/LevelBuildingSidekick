using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Generator3DPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Generator3DPanel, VisualElement.UxmlTraits> { }

    public Generator3DPanel() { }

    public Generator3DPanel(LBSLevelData levelData)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);
    }
}
