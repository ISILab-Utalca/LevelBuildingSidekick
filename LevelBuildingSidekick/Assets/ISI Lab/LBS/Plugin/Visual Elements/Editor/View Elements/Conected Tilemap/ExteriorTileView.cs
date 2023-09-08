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
    // VisualElements
    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;

    private static VisualTreeAsset view;

    public ExteriorTileView(List<string> connections)
    {
        if (view == null)
        {
            ExteriorTileView.view = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConnectedTile");
        }
        ExteriorTileView.view.CloneTree(this);

        this.SetMargins(0);
        this.SetPaddings(0);
        this.SetBorder(Color.black, 1);

        // conecctions
        left = this.Q<VisualElement>("Left");
        right = this.Q<VisualElement>("Right");
        top = this.Q<VisualElement>("Top");
        bottom = this.Q<VisualElement>("Bottom");
        border = this.Q<VisualElement>("Border");


        SetConnections(connections.ToArray());
    }

    public void SetBackgroundColor(Color color)
    {
        border.style.backgroundColor = color;
    }


    public void SetConnections(string[] tags)
    {
        var tts = LBSAssetsStorage.Instance.Get<LBSIdentifier>();

        if (!string.IsNullOrEmpty(tags[0]))
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
    }
}
