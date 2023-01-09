using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TileView : LBSGraphElement
{
    // Constructors
    public TileView(LBSGraphView root) : base(root) { }

    // Methods
    public abstract override void OnDelete();

    public void SetView(string[] tags)
    {
        SetView(tags[0], tags[1], tags[2], tags[3]);
    }

    public abstract void SetView(string top, string right, string bottom, string left);

    public void SetBackgorundColor(Color color)
    {
        this.style.backgroundColor = color;
    }

    public void SetSize(int x, int y)
    {
        this.style.width = this.style.maxWidth = this.style.minWidth = x;
        this.style.height = this.style.maxHeight = this.style.minHeight = y;
    }
}

public class TileStyle : ScriptableObject
{

}