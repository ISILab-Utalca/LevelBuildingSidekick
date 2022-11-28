using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileWFC_View : LBSGraphElement //c4
{
    public TileWFC_struct Data;

    public VisualElement left;
    public VisualElement right;
    public VisualElement top;
    public VisualElement bottom;
    public override void OnDelete()
    {
        throw new System.NotImplementedException();
    }

    public TileWFC_View(TileWFC_struct tile, LBSGraphView root) : base(root)
    {
        Data = tile;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileWFC");
        visualTree.CloneTree(this);

        left = this.Q<VisualElement>(name: "Left");
        right = this.Q<VisualElement>(name: "Right");
        top = this.Q<VisualElement>(name: "Top");
        bottom = this.Q<VisualElement>(name: "Bottom");
    }

    public void ShowDir(Vector2Int dir)
    {
        var angle = Vector2.SignedAngle(Vector2.right, dir) % 360;
        if (angle < 0)
            angle = 360 + angle;

        if (angle < 45 || angle > 315)
        {
            right.visible = true;
        }
        else if (angle > 45 && angle <= 135)
        {
            bottom.visible = true;
        }
        else if (angle > 135 && angle <= 225)
        {
            left.visible = true;
        }
        else if (angle > 225 && angle <= 315)
        {
            top.visible = true;
        }
    }

    public void SetRelationColor(Color color)
    {
        left.style.backgroundColor = color;
        right.style.backgroundColor = color;
        top.style.backgroundColor = color;
        bottom.style.backgroundColor = color;
    }

    public void HideAllRelations()
    {
        left.visible = false;
        right.visible = false;
        top.visible = false;
        bottom.visible = false;
    }

    public void SetColor(Color color)
    {
        this.style.backgroundColor = color;
    }

    private void SetPadding(int value)
    {
        this.style.paddingBottom = this.style.paddingLeft = this.style.paddingRight = this.style.paddingTop = value;
    }

    private void SetBorderWidth(int value)
    {
        this.style.borderBottomWidth = this.style.borderRightWidth = this.style.borderLeftWidth = this.style.borderTopWidth = value;
    }

    public void SetSize(int size)
    {
        SetSize(size, size);
    }

    public void SetSize(int x, int y)
    {
        this.style.width = this.style.maxWidth = this.style.minWidth = x;
        this.style.height = this.style.maxHeight = this.style.minHeight = y;
    }
}
