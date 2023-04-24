using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacteristicBaseView : VisualElement
{
    public CharacteristicBaseView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicBaseView");
        visualTree.CloneTree(this);
    }
}
