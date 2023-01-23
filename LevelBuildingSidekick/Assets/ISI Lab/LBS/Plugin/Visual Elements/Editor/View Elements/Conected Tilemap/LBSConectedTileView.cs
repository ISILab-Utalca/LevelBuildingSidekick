using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSConectedTileView : GraphElement
{
    private ConnectedTile data;

    public ConnectedTile Data => data;

    // VisualElements
    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;

    public LBSConectedTileView()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConnectedTile");
        visualTree.CloneTree(this);

        // conecctions
        left = this.Q<VisualElement>("Left");
        right = this.Q<VisualElement>("Right");
        top = this.Q<VisualElement>("Top");
        bottom = this.Q<VisualElement>("Bottom");
    }


}
