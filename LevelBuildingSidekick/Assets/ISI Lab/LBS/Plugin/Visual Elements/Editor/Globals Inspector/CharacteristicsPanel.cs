using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacteristicsPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CharacteristicsPanel, VisualElement.UxmlTraits> { }
    #endregion

    public CharacteristicsPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsPanel");
        visualTree.CloneTree(this);
    }

    public void SetInfo(Bundle_Old target)
    {

    }
}
