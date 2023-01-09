using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BrushSettings : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BrushSettings, VisualElement.UxmlTraits> { }

    public TextField nameList;
    public DropdownField brushStamp;
    public SliderInt sizeBrushSlider;
    public IntegerField sizeBrushIntField;

    public BrushSettings()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BrushSettings");
        visualTree.CloneTree(this);

        nameList = this.Q<TextField>(name: "NameList");
        brushStamp = this.Q<DropdownField>(name: "BrushStamp");
        sizeBrushSlider = this.Q<SliderInt>(name: "SizeB1");
        sizeBrushIntField = this.Q<IntegerField>(name: "SizeB2");

    }

}
