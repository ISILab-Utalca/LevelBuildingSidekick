using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawTool : GraphElement
{
    public new class UxmlFactory : UxmlFactory<ExternalBounds, UxmlTraits> { }

    private Painter2D paint2D;
    private Vector2 initialPos, endPos;
    private Rect @internal, external;

    public DrawTool()
    {
        this.style.width = 50; // (!) deshardcodear
        this.style.height = 50;

        this.@internal = new Rect(new Vector2(-10, -10), new Vector2(20, 20));
        this.external = new Rect(new Vector2(-20, -20), new Vector2(40, 40));

        this.generateVisualContent += OnGenerateVisualContent;
    }

    public DrawTool(Vector2 initialPos, Vector2 endPos)
    {
        this.initialPos = initialPos;
        this.endPos = endPos;

        this.generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        // Init
        paint2D = mgc.painter2D;

        DrawLine(initialPos, endPos,
            new Color(1, 0, 0, 0.05f),
            new Color(1, 0, 0, 1f),
            0
            );
    }

    private void DrawHoleyShape(List<List<Vector2>> points, Color color, Color border, float stroke = 1)
    {
        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        foreach (var path in points)
        {
            paint2D.MoveTo(path[0]);
            for (int i = 1; i < path.Count; i++)
            {
                paint2D.LineTo(path[i]);
            }

            paint2D.ClosePath();
        }
        paint2D.lineWidth = stroke;
        paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();

    }

    private void DrawLine(Vector2 iniPos, Vector2 endPos, Color color, Color border, float stroke = 1)
    {
        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        
        paint2D.MoveTo(iniPos);
            
        paint2D.LineTo(endPos);
            

        paint2D.ClosePath();
        
        paint2D.lineWidth = stroke;
        //paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();
    }
}
