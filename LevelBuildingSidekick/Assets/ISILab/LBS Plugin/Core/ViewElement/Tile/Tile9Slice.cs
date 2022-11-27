using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile9Slice : TileView
{
    // tabla de asignacion de partes
    //  |TR TC TL|    |0 1 2|
    //  |ML MC MR| -> |3 4 5|
    //  |BL BC BR|    |6 7 8|

    private VisualElement topRight;
    private VisualElement topCenter;
    private VisualElement topLeft;
    private VisualElement middleRight;
    private VisualElement middleCenter;
    private VisualElement middelLeft;
    private VisualElement bottomRight;
    private VisualElement bottomCenter;
    private VisualElement bottomLeft;
    private VisualElement[] all;

    private static readonly Color basic = Color.black;

    public Tile9Slice(LBSGraphView rootView) : base(rootView)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Tile9Slide");
        visualTree.CloneTree(this);

        all = new VisualElement[9];
        all[0] =  topLeft = this.Q<VisualElement>("TopLeft");
        all[1] = topCenter = this.Q<VisualElement>("TopCenter");
        all[2] = topRight = this.Q<VisualElement>("TopRight");
        all[3] = middelLeft = this.Q<VisualElement>("MiddleLeft");
        all[4] = middleCenter = this.Q<VisualElement>("MiddleCenter");
        all[5] = middleRight = this.Q<VisualElement>("MiddleRight");
        all[6] = bottomLeft = this.Q<VisualElement>("MiddleLeft");
        all[7] = bottomCenter = this.Q<VisualElement>("MiddleCenter");
        all[8] = bottomRight = this.Q<VisualElement>("MiddleRight");
    }

    public override void OnDelete()
    {
        throw new System.NotImplementedException();
    }

    public void SetBackgroundColor(Color color)
    {
        for (int i = 0; i < all.Length; i++)
        {
            all[i].style.backgroundColor = color;
        }
    }

    public override void SetView(string top, string right, string bottom, string left)
    {
        throw new System.NotImplementedException();
    }

    public override void SetView(LBSTag top, LBSTag right, LBSTag bottom, LBSTag left)
    {
        throw new System.NotImplementedException();
    }
}
