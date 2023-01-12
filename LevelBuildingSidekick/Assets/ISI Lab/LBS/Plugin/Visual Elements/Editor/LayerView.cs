using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayerView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LayerView, VisualElement.UxmlTraits> { }

    private Label layerName;

    public LayerView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayerView"); // Editor
        visualTree.CloneTree(this);

        // LayerName
        layerName= this.Q<Label>("LayerName");
    }

    public void SetName(string name)
    {
        layerName.text = name;
    }
}
