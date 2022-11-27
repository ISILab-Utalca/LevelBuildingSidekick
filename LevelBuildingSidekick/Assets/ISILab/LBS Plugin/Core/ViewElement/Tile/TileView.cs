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
    public abstract void SetView(string top, string right, string bottom, string left);
    public abstract void SetView(LBSTag top, LBSTag right, LBSTag bottom, LBSTag left);

    public void SetBackgorundColor(Color color)
    {
        this.style.backgroundColor = color;
    }
}
