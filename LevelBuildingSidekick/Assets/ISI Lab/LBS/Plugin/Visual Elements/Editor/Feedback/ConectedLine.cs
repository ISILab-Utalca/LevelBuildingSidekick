using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConectedLine : Feedback
{
    public ConectedLine(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }
    public ConectedLine() : base()
    {
        this.StartOffset = (TeselationSize.Multiply(0.5f));
        this.EndOffset = (TeselationSize.Multiply(0.5f));
    }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var line = new List<Vector2>() { startPosition, endPosition };
        painter.DrawPoligon(line, new Color(0, 0, 0, 0), currentColor, 4, false);
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

            startPosition = (startPosition * TeselationSize) + StartOffset;
            endPosition = (endPosition * TeselationSize) + EndOffset;
        }

        this.MarkDirtyRepaint();
    }
}