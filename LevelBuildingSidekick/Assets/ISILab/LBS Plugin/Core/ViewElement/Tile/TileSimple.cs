using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileSimple : TileView
{
    public TileData Data;

    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;
    private Label cordsLabel;

    public TileSimple(TileData data, LBSGraphView root) : base(root)
    {
        this.Data = data;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileSimple");
        visualTree.CloneTree(this);

        left = this.Q<VisualElement>(name: "Left");
        right = this.Q<VisualElement>(name: "Right");
        top = this.Q<VisualElement>(name: "Top");
        bottom = this.Q<VisualElement>(name: "Bottom");
        border = this.Q<VisualElement>("Border");
        cordsLabel = this.Q<Label>("CordsLabel");
    }

    public override void OnDelete()
    {
        throw new System.NotImplementedException();
    }

    public override void SetView(string top, string right, string bottom, string left)
    {
        if (top != "")
        {
            this.top.visible = false;
        }
        if (right != "")
        { 
            this.right.visible = false;
        }
        if (bottom != "")
        {
            this.bottom.visible = false;
        }
        if (bottom != "")
        { 
            this.bottom.visible = false;
        }


    }

    public override void SetView(LBSTag top, LBSTag right, LBSTag bottom, LBSTag left)
    {
        throw new System.NotImplementedException();
    }


        

}

[CreateAssetMenu(menuName = "ISILab/LBS Plugin/Tile style")]
public class tileSimpleStyle : TileStyle
{
    public List<Style> styles;

    [System.Serializable]
    public struct Style
    {
        public string value;
        public Color color;
    }

}