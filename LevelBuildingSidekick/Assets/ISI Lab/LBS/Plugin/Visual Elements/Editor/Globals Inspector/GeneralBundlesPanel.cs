using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralBundlesPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<GeneralBundlesPanel, VisualElement.UxmlTraits> { }
    #endregion

    private ObjectField iconField;
    private TextField textField;
    private ColorField colorField;

    public GeneralBundlesPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GeneralBundlesPanel");
        visualTree.CloneTree(this);

        this.iconField = this.Q<ObjectField>("IconField");
        this.textField = this.Q<TextField>("NameField");
        this.colorField = this.Q<ColorField>("ColorField");
    }

    public void SetInfo(Bundle target)
    {

    }
}
