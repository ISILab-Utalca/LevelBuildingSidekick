using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class TagView : VisualElement
{
    public LBSIdentifier tagSO;

    // Visual element
    private TextField textfield;

    public TagView(LBSIdentifier tag)
    {
        this.tagSO = tag;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
        visualTree.CloneTree(this);

        // TextField
        textfield = this.Q<TextField>();
        textfield.RegisterCallback<ChangeEvent<string>>(e => OnTextChange(e.newValue));
    }

    public void OnTextChange(string value)
    {
        tagSO.Label = value;
        AssetDatabase.SaveAssets();
    }
}
