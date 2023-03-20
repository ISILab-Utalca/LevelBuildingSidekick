using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetOptions : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TargetOptions, VisualElement.UxmlTraits> { }

    public TargetOptions()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TargetOptionsUXML");
        visualTree.CloneTree(this);

    }
}
