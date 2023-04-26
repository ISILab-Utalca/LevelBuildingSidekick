using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacteristicsBaseView : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CharacteristicsBaseView, VisualElement.UxmlTraits> { }
    #endregion

    public CharacteristicsBaseView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsBaseView");
        visualTree.CloneTree(this);

    }
}
