using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    public AIPanel() { }

    public AIPanel(LBSLevelData levelData)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);
    }
}
