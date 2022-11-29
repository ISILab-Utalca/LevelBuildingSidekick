using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(WFCWindow), ID, "WFC Tools Extra", "-", defaultDisplay = true)]
public class WFCExtraTools : Overlay
{
    private const string ID = "WFCExtraToolsOverlay";

    private Button fillButton;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("WFCExtraTools");
        visualTree.CloneTree(root);

        fillButton = root.Q<Button>("FillButton");
        fillButton.clicked += () =>
        {

        };

        return root;
    }
}