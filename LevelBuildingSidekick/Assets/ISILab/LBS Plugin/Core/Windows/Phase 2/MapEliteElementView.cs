using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEliteElementView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<MapEliteElementView, VisualElement.UxmlTraits> { }

    public Button btn;
    public VisualElement icon;
    public VisualElement loading;

    public MapEliteElementView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteElementUXML");
        visualTree.CloneTree(this);
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("RotateUSS");
        this.styleSheets.Add(styleSheet);


        this.btn = this.Q<Button>();
        this.icon = this.Q<VisualElement>("Loading");
        this.loading = this.Q<VisualElement>("Icon");

    }

}
