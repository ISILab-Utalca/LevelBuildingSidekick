using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacteristicsBaseView : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CharacteristicsBaseView, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private Bundle bundle;

    public CharacteristicsBaseView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsBaseView");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
    }

    public void SetContent(Bundle bundle,VisualElement content)
    {
        this.bundle = bundle;
        this.content.Add(content);
    }
}
