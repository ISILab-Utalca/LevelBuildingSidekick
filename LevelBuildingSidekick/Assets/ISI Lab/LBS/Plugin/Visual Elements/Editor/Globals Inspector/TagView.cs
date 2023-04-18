using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class TagView : VisualElement
{
    public LBSIdentifier target;

    // Visual element
    private TextField textfield;

    // Visual element
    private ColorField colorfield;

    public TagView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagView"); // Editor
        visualTree.CloneTree(this);

        // TextField
        textfield = this.Q<TextField>();
        textfield.RegisterCallback<ChangeEvent<string>>(e => OnTextChange(e.newValue));

        // ColorField
        colorfield = this.Q<ColorField>("ColorField");
        colorfield?.RegisterCallback<ChangeEvent<Color>>(e => OnColorChange(e.newValue));
    }

    public void SetInfo(LBSIdentifier target)
    {
        this.target = target;
        textfield.value = target.Label;
        colorfield.value = target.Color;
    }

    public void OnTextChange(string value)
    {
        target.Label = value;
        AssetDatabase.SaveAssets();
    }

    public void OnColorChange(Color color)
    {
        target.Color = color;
        AssetDatabase.SaveAssets();
    }
}
