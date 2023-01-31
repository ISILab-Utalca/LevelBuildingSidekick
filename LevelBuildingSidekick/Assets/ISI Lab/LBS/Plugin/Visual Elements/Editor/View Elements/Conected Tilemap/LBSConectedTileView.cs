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

    public LBSConectedTileView(ConnectedTile connectedTile)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConnectedTile");
        visualTree.CloneTree(this);

        // conecctions
        left = this.Q<VisualElement>("Left");
        right = this.Q<VisualElement>("Right");
        top = this.Q<VisualElement>("Top");
        bottom = this.Q<VisualElement>("Bottom");
        border = this.Q<VisualElement>("Border");

        this.data = connectedTile;

        SetConnections(data.Connections);
    }

    public void SetBackgroundColor(Color color)
    {
        border.style.backgroundColor = color;
    }

    public virtual void SetConnections(string[] tags)
    {
        Debug.LogWarning("Implementar");
    }

}
