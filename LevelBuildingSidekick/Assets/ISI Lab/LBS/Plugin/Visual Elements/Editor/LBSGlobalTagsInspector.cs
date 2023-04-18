using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSGlobalTagsInspector : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSGlobalTagsInspector, VisualElement.UxmlTraits> { }
    #endregion

    public LBSGlobalTagsInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSGlobalTagsInspector");
        visualTree.CloneTree(this);

        var list = this.Q<CustomList>();

        var bundles = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();
        foreach (var bundle in bundles)
        {
            var bv = new TagBundleView(bundle);
            list.AddElement(bv);
        }

    }
}
