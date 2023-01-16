using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayerView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LayerView, VisualElement.UxmlTraits> { }

    private Label layerName;
    private VisualElement layerIcon;

    public LayerView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayerView"); // Editor
        visualTree.CloneTree(this);

        // LayerName
        layerName= this.Q<Label>("LayerName");

        // LayerIcon
        layerIcon = this.Q<VisualElement>("LayerIcon");
    }

    public void SetName(string name)
    {
        layerName.text = name;
    }

    public void SetIcon(string name)
    {
        var texture = Resources.Load<Texture2D>(name);
        layerIcon.style.backgroundImage = texture;
    }
}
