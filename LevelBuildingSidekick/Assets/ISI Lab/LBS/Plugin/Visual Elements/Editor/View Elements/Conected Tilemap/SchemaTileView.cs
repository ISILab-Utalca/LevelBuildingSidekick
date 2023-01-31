using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class SchemaTileView : GraphElement
{
    private ConnectedTile data;

    public ConnectedTile Data => data;

    // VisualElements
    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;

    public SchemaTileView(ConnectedTile connectedTile)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("SchemaTile");
        visualTree.CloneTree(this);

        this.SetMargins(0);
        this.SetPaddings(0);
        this.SetBorder(Color.black, 1);

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
        right.style.backgroundColor = color;
        top.style.backgroundColor = color;
        left.style.backgroundColor = color;
        bottom.style.backgroundColor = color;
    }

    public void SetConnections(string[] tags)
    {
        right.style.display = (tags[0].Equals("Door"))? DisplayStyle.Flex : DisplayStyle.None;
        top.style.display = (tags[1].Equals("Door")) ? DisplayStyle.Flex : DisplayStyle.None;
        left.style.display = (tags[2].Equals("Door")) ? DisplayStyle.Flex : DisplayStyle.None;
        bottom.style.display = (tags[3].Equals("Door")) ? DisplayStyle.Flex : DisplayStyle.None;

        border.style.borderRightWidth = (tags[0].Equals("Empty")) ? 0f : 8f;
        border.style.borderTopWidth = (tags[1].Equals("Empty")) ? 0f : 8f;
        border.style.borderLeftWidth = (tags[2].Equals("Empty")) ? 0f : 8f;
        border.style.borderBottomWidth = (tags[3].Equals("Empty")) ? 0f : 8f;
    }

}
