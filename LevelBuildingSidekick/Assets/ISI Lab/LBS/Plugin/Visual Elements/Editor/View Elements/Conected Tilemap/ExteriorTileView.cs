using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class ExteriorTileView : GraphElement
{
    private ConnectedTile data;

    public ConnectedTile Data => data;

    // VisualElements
    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;

    public ExteriorTileView(ConnectedTile connectedTile)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConnectedTile");
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
    }

    public void SetConnections(string[] tags)
    {
        /*
        right.style.display = (!tags[0].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        top.style.display = (!tags[1].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        left.style.display = (!tags[2].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        bottom.style.display = (!tags[3].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        */

        var tts = Utility.DirectoryTools.GetScriptables<LBSIdentifier>().ToList();
        if(!string.IsNullOrEmpty(tags[0]))
        {
            right.style.backgroundColor = tts.Find(t => t.Label.Equals(tags[0])).Color;
            right.style.display = DisplayStyle.Flex;
        }
        else
        {
            right.style.display = DisplayStyle.None;
        }

        if (!string.IsNullOrEmpty(tags[1]))
        {
            top.style.backgroundColor = tts.Find(t => t.Label.Equals(tags[1])).Color;
            top.style.display = DisplayStyle.Flex;
        }
        else
        {
            top.style.display = DisplayStyle.None;
        }

        if (!string.IsNullOrEmpty(tags[2]))
        {
            left.style.backgroundColor = tts.Find(t => t.Label.Equals(tags[2])).Color;
            left.style.display = DisplayStyle.Flex;
        }
        else
        {
            left.style.display = DisplayStyle.None;
        }

        if (!string.IsNullOrEmpty(tags[3]))
        {
            bottom.style.backgroundColor = tts.Find(t => t.Label.Equals(tags[3])).Color;
            bottom.style.display = DisplayStyle.Flex;
        }
        else
        {
            bottom.style.display = DisplayStyle.None;
        }
        //top.style.backgroundColor = (!tags[1].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        //left.style.backgroundColor = (!tags[2].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
        //bottom.style.backgroundColor = (!tags[3].Equals("")) ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
