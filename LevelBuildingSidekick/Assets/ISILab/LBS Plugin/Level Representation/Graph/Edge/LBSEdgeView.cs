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
    protected LBSEdgeData data; // (??)

    protected LBSNodeView nv1, nv2;
    protected GraphView root;

    public LBSEdgeData Data => data;

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

    public LBSDotedEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root) : base(nv1, nv2, root)
    {
    }

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

    public override void OnDelete()
    {
        elements.ForEach(e => root.RemoveElement(e));
        elements = new List<GraphElement>();
    }
}

public class LBSLineEdgeView : LBSEdgeView
{
    public Line line;

    public LBSLineEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root) : base(nv1, nv2, root)
    {
    }


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

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {

    }
}

public class Dot : GraphElement
{
    public Dot(int size)
    {
        var box = new Box();
        box.style.minHeight = box.style.minWidth = box.style.maxHeight = box.style.maxWidth = size;
        box.style.backgroundColor = Color.white;
        this.Add(box);
    }
}