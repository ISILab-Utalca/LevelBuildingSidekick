using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileSimple : TileView
{
    private VisualTreeAsset visualTree;

    private VisualElement left;
    private VisualElement right;
    private VisualElement top;
    private VisualElement bottom;
    private VisualElement border;
    private Label cordsLabel;

    public TileSimple() : base(null) // (!!) ver si este null no da problemas
    {

        left = this.Q<VisualElement>(name: "Left");
        right = this.Q<VisualElement>(name: "Right");
        top = this.Q<VisualElement>(name: "Top");
        bottom = this.Q<VisualElement>(name: "Bottom");
        border = this.Q<VisualElement>("Border");
        cordsLabel = this.Q<Label>("CordsLabel");
    }

    public override void LoadVisual()
    {
        if(!visualTree)
        {
            visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileSimple");
        }
        visualTree.CloneTree(this);
    }

    public override void OnDelete()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetView(string top, string right, string bottom, string left)
    {
        if (top == "")
        {
            this.top.visible = false;
        }
        else
        {
            this.top.style.backgroundColor = WFCTools.GetColor(top);
        }
        if (right == "")
        { 
            this.right.visible = false;
        }
        else
        {
            this.right.style.backgroundColor = WFCTools.GetColor(right);
        }
        if (bottom == "")
        {
            this.bottom.visible = false;
        }
        else
        {
            this.bottom.style.backgroundColor = WFCTools.GetColor(bottom);
        }
        if (left == "")
        { 
            this.left.visible = false;
        }
        else
        {
            this.left.style.backgroundColor = WFCTools.GetColor(left);
        }
    }

}

[CreateAssetMenu(menuName = "ISILab/LBS plugin/Tile style")]
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