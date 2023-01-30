using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Generator;
using LBS.Components;

public class Generator3DPanel : VisualElement
{
    Generator generator;
    ClassDropDown dropDown;

    public Generator Generator
    {
        get => generator;
        set => generator = value;
    }

    public new class UxmlFactory : UxmlFactory<Generator3DPanel, VisualElement.UxmlTraits> { }

    public Generator3DPanel() { }

    public Generator3DPanel(LBSLevelData data)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);

        dropDown = this.Q<ClassDropDown>();
        dropDown.Label = "Gennerator";
        dropDown.Type = typeof(Generator);


    }
}
