using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LBS;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Newtonsoft.Json;
using LBS.Graph;

public abstract class LBSEdgeView : LBSGraphElement
{
    protected LBSEdgeDataOld data; // (??)

    protected LBSNodeView nv1, nv2;
    protected GraphView root;

    public LBSEdgeDataOld Data => data;

    /// <summary>
    /// Default base class constructor for LBSEdgeView class.
    /// </summary>
    public LBSEdgeView() : base() { }

    /// <summary>
    /// Constructor for the LBSEdgeView class.
    /// </summary>
    /// <param name="nv1">First node view in the edge.</param>
    /// <param name="nv2">Second node view in the edge.</param>
    /// <param name="root">Root graph view.</param>
    public LBSEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root) : base(root)
    {
        this.nv1 = nv1;
        this.nv2 = nv2;
        this.root = root;

        capabilities |= Capabilities.Selectable | Capabilities.Deletable;

        ActualizeView();

        nv1.OnMoving += () =>
        {
            ActualizeView();
            nv1.BringToFront();
        };

        nv2.OnMoving += () =>
        {
            ActualizeView();
            nv2.BringToFront();
        };
    }

    public abstract void ActualizeView();
}

public class LBSDotedEdgeView : LBSEdgeView
{
    private static float dist = 10f;
    public List<GraphElement> elements = new List<GraphElement>();

    /// <summary>
    /// Constructor of the class LBSDotedEdgeView.
    /// </summary>
    /// <param name="nv1">fFirst node view in the edge.</param>
    /// <param name="nv2">Second node view in the edge<./param>
    /// <param name="root">Root graph view.</param>
    public LBSDotedEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root) : base(nv1, nv2, root)
    {
    }

    public override void LoadVisual()
    {
    }

    /// <summary>
    /// Uupdates the view of an edge. It does so by removing 
    /// all dots that make up the edge, and then adding new 
    /// ones based on the current positions of the two nodes 
    /// that the edge connects.
    /// </summary>
    public override void ActualizeView()
    {
        var pos1 = this.nv1.GetPosition().center;
        var pos2 = this.nv2.GetPosition().center;

        var vec = (pos2 - pos1).normalized;
        var num = Vector2.Distance(pos1, pos2) / dist;

        elements.ForEach(e => root.RemoveElement(e));
        elements = new List<GraphElement>();

        for (int i = 0; i < num; i++)
        {
            var dot = new Dot(5);
            var p = pos1 + (i * vec * dist);
            dot.SetPosition(new Rect(p.x, p.y, 5, 5));
            elements.Add(dot);
            root.AddElement(dot);
            dot.SendToBack();
        }
    }

    /// <summary>
    /// Remove the content from the elements list and replace the list with a new one.
    /// </summary>
    public override void OnDelete()
    {
        elements.ForEach(e => root.RemoveElement(e));
        elements = new List<GraphElement>();
    }
}

public class LBSLineEdgeView : LBSEdgeView
{
    public Line line;

    /// <summary>
    /// Cosntructor for the LBSLineEdgeView class.
    /// </summary>
    /// <param name="nv1">First node view in the edge.</param>
    /// <param name="nv2">Second node view in the edge.</param>
    /// <param name="root">Root graph view.</param>
    public LBSLineEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root) : base(nv1, nv2, root)
    {
    }

    public override void LoadVisual()
    {
    }

    /// <summary>
    /// Update the view.
    /// </summary>
    public override void ActualizeView()
    {
        var pos1 = this.nv1.GetPosition().center;
        var pos2 = this.nv2.GetPosition().center;
    }

    public override void OnDelete()
    {
        throw new System.NotImplementedException();
    }
}


public class Line : GraphElement
{
    Painter2D painter;

    public Line()
    {

    }
}

public class Dot : GraphElement
{
    /// <summary>
    /// Constructor for the Dot class with the specified size.
    /// </summary>
    /// <param name="size">The size of the dot.</param>
    public Dot(int size)
    {
        var box = new Box();
        box.style.minHeight = box.style.minWidth = box.style.maxHeight = box.style.maxWidth = size;
        box.style.backgroundColor = Color.white;
        this.Add(box);
    }
}