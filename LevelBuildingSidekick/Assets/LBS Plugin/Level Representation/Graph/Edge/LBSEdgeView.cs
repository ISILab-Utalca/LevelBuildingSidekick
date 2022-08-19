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
    public LBSNodeView nv1, nv2;
    public Painter2D painter;

    public LBSEdgeView(LBSNodeView nv1, LBSNodeView nv2)
    {
        this.nv1 = nv1;
        this.nv2 = nv2;

        capabilities |= Capabilities.Selectable | Capabilities.Deletable;
        generateVisualContent += OnGenerateVisualContent;
        
    }

    public void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        if (nv1 != null || nv2 != null)
            return;

        painter = mgc.painter2D;
        painter.strokeColor = Color.white;
        painter.lineWidth = 2f;
        painter.lineCap = LineCap.Round;
        var p1 = nv1.GetPosition();
        var p2 = nv2.GetPosition();

        painter.BeginPath();
        painter.MoveTo(p1.center);
        painter.LineTo(p2.center);
        painter.Stroke();
    }

}

public class LBSProxyEdge : GraphElement
{
    public Vector2 nv1, nv2;
    public Painter2D painter;

    public LBSProxyEdge(Vector2 nv1, Vector2 nv2)
    {
        this.nv1 = nv1;
        this.nv2 = nv2;
    }

    public void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        painter = mgc.painter2D;
    }

    public void UpdateDraw(Vector2 p1, Vector2 p2)
    {
        painter.strokeColor = Color.white;
        painter.lineWidth = 2f;
        painter.lineCap = LineCap.Round;

        painter.BeginPath();
        painter.MoveTo(p1);
        painter.LineTo(p2);
        painter.Stroke();
    }
}