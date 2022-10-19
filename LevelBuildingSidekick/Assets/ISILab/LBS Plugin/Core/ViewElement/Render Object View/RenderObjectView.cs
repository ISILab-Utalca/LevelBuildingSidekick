using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RenderObjectView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<RenderObjectView, UxmlTraits> { }

    public void CreateGUI()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("RenderObjectUXML");
        visualTree.CloneTree(this);
    }

    public void SetDimension(int x, int y)
    {

    }

}
