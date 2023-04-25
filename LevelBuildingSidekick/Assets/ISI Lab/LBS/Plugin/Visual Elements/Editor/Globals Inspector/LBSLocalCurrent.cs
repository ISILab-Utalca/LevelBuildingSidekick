using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalCurrent : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalCurrent, VisualElement.UxmlTraits> { }
    #endregion

    public LBSLocalCurrent()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalCurrent");
        visualTree.CloneTree(this);
    }
}
