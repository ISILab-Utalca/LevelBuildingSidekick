using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionCharView : VisualElement
{
    private Button8Connected connected8;

    public ConnectionCharView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConnectionCharView");
        visualTree.CloneTree(this);

        connected8 = this.Q<Button8Connected>();
    }
}
