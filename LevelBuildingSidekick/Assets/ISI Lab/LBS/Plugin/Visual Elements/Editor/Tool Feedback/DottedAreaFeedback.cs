using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DottedAreaFeedback : Feedback
{
    public DottedAreaFeedback(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }

    public DottedAreaFeedback() : base()
    {
        this.EndOffset = TeselationSize;
    }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var color = currentColor * new Color(1, 1, 1, 0.5f);

        var points = new List<Vector2>()
        {
            new Vector2(startPosition.x, startPosition.y),
            new Vector2(startPosition.x, endPosition.y),
            new Vector2(endPosition.x, endPosition.y),
            new Vector2(endPosition.x, startPosition.y),
        };
        painter.DrawDottedPolygon(points, color, 4, true);
    }

    public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
    {
        startPosition = new Vector2Int(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
        endPosition = new Vector2Int(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));

        if (fixToTeselation)
        {
            startPosition = CalcFixTeselation(startPosition);
            endPosition = CalcFixTeselation(endPosition);

            startPosition = (startPosition * TeselationSize) + StartOffset;
            endPosition = ((endPosition) * TeselationSize) + TeselationSize;
        }

        this.MarkDirtyRepaint();
    }

    internal void SetColor(Color color)
    {
        currentColor = color;
        this.MarkDirtyRepaint();
    }
}