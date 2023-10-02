using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class SchemaTileView : GraphElement
{
    #region VIEW FIELDS
    private static VisualTreeAsset view;

    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;
    #endregion

    public SchemaTileView()
    {
        if (view == null)
        {
            SchemaTileView.view = DirectoryTools.SearchAssetByName<VisualTreeAsset>("SchemaTileView");
        }
        SchemaTileView.view.CloneTree(this);

        left = this.Q<VisualElement>("Left");
        right = this.Q<VisualElement>("Right");
        top = this.Q<VisualElement>("Top");
        bottom = this.Q<VisualElement>("Bottom");
        border = this.Q<VisualElement>("Border");

        this.SetMargins(0);
        this.SetPaddings(0);
        this.SetBorderRadius(0);
        this.SetBorder(Color.black, 1);
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
        right.SetDisplay(tags[0].Equals("Door"));
        top.SetDisplay(tags[1].Equals("Door"));
        left.SetDisplay(tags[2].Equals("Door"));
        bottom.SetDisplay(tags[3].Equals("Door"));

        border.style.borderRightWidth = (tags[0].Equals("Empty")) ? 0f : 8f;
        border.style.borderTopWidth = (tags[1].Equals("Empty")) ? 0f : 8f;
        border.style.borderLeftWidth = (tags[2].Equals("Empty")) ? 0f : 8f;
        border.style.borderBottomWidth = (tags[3].Equals("Empty")) ? 0f : 8f;
    }

}
