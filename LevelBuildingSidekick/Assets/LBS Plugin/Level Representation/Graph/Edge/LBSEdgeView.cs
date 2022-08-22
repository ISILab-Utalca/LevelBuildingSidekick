using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Newtonsoft.Json;
using LevelBuildingSidekick.Graph;

public class LBSEdgeView : GraphElement
{
    private static float dist = 10f;

    private LBSNodeView nv1, nv2;
    private List<GraphElement> elements = new List<GraphElement>();
    private GraphView root;

    public LBSEdgeView(LBSNodeView nv1, LBSNodeView nv2, LBSGraphView root)
    {
        this.nv1 = nv1;
        this.nv2 = nv2;
        this.root = root;

        capabilities |= Capabilities.Selectable | Capabilities.Deletable;

        UpdateDots();
        nv1.OnMoving += ()=>
        {
            UpdateDots();
            nv1.BringToFront();
        };

        nv2.OnMoving += () =>
        {
            UpdateDots();
            nv2.BringToFront();
        };

    }


    public void UpdateDots()
    {
        var pos1 = this.nv1.GetPosition().center;
        var pos2 = this.nv2.GetPosition().center;

        var vec = (pos2 - pos1).normalized;
        var num = Vector2.Distance(pos1,pos2)/dist;

        elements.ForEach(e => root.RemoveElement(e));
        elements = new List<GraphElement>();

        for (int i = 0; i < num; i++)
        {
            var dot = new Dot(5);
            var p = pos1 + (i * vec * dist);
            dot.SetPosition(new Rect(p.x,p.y,5,5));
            elements.Add(dot);
            root.AddElement(dot);

        } 
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