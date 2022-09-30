using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BrushView : Button
{
    public new class UxmlFactory : UxmlFactory<BrushView, VisualElement.UxmlTraits> { }

    private StampPresset presset;

    public VisualElement icon;
    public Label label;

    public StampPresset Presset => presset;

    public BrushView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("IconBtnUXML");
        visualTree.CloneTree(this);
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
        this.styleSheets.Add(styleSheet);

        this.icon = this.Q<VisualElement>("image");
        this.label = this.Q<Label>();
    }

    public BrushView(StampPresset presset)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("IconBtnUXML");
        visualTree.CloneTree(this);
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
        this.styleSheets.Add(styleSheet);

        this.icon = this.Q<VisualElement>("image");
        this.label = this.Q<Label>();

        SetValue(presset);
    }

    public void SetValue(StampPresset presset)
    {
        this.presset = presset;
        this.icon.style.backgroundImage = presset.Icon;
        this.label.text = presset.name;
    }
}
