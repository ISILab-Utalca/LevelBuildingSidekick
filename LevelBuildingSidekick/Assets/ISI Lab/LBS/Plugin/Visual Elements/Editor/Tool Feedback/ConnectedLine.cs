using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectedLine : Feedback
{
    public ConnectedLine(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }
    public ConnectedLine() : base()
    {
        this.StartOffset = (TeselationSize.Multiply(0.5f));
        this.EndOffset = (TeselationSize.Multiply(0.5f));
    }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var line = new List<Vector2>() { startPosition, endPosition };
        painter.DrawPolygon(line, new Color(0, 0, 0, 0), currentColor, 4, false);
        painter.DrawCircle(startPosition, 16, currentColor);
        painter.DrawCircle(endPosition, 16, currentColor);
    }

    public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
    {
        startPosition = p1;
        endPosition = p2;

        if (fixToTeselation)
        {
            startPosition = CalcFixTeselation(startPosition);
            endPosition = CalcFixTeselation(endPosition);


            startPosition = (startPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
            endPosition = (endPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
        }

        this.MarkDirtyRepaint();
    }
}

public class ConnectedConrnerLine : Feedback
{
    public bool LeftSide = false;

    public Vector2Int cornerPoint;

    public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
    {
        startPosition = p1;
        endPosition = p2;

        cornerPoint = LeftSide ?
                new Vector2Int(startPosition.x, endPosition.y) :
                new Vector2Int(endPosition.x, startPosition.y);

        if (fixToTeselation)
        {
            startPosition = CalcFixTeselation(startPosition);
            endPosition = CalcFixTeselation(endPosition);
            cornerPoint = CalcFixTeselation(cornerPoint);

            startPosition = (startPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
            endPosition = (endPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
            cornerPoint = (cornerPoint * TeselationSize) + TeselationSize.Multiply(0.5f);
        }

        this.MarkDirtyRepaint();
    }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var line = new List<Vector2>() { startPosition, cornerPoint, endPosition };
        painter.DrawPolygon(line, new Color(0, 0, 0, 0), currentColor, 4, false);
        painter.DrawCircle(startPosition, 16, currentColor);
        painter.DrawCircle(endPosition, 16, currentColor);
    }
}

