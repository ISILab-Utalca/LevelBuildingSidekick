using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ExternalBounds : GraphElement
{
    public new class UxmlFactory : UxmlFactory<ExternalBounds, UxmlTraits> { }

    private Painter2D paint2D;
    private Rect @internal, external;

    public ExternalBounds()
    {
        this.style.width = 50; // (!) deshardcodear
        this.style.height = 50;

        this.@internal = new Rect(new Vector2(-10, -10),new Vector2( 20, 20));
        this.external = new Rect(new Vector2(-20, -20), new Vector2(40, 40));

        this.generateVisualContent += OnGenerateVisualContent;
    }

    public ExternalBounds(Rect external,Rect @internal)
    {
        this.@internal = @internal;
        this.external = external;

        this.generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        // Init
        paint2D = mgc.painter2D;

        DrawHoleyShape(
             new List<List<Vector2>>()
             {
                new List<Vector2>() {
                    new Vector2(@internal.xMin, @internal.yMin),
                    new Vector2(@internal.xMax, @internal.yMin),
                    new Vector2(@internal.xMax, @internal.yMax),
                    new Vector2(@internal.xMin, @internal.yMax),
                },
                new List<Vector2>() {
                    new Vector2(external.xMin, external.yMin),
                    new Vector2(external.xMax, external.yMin),
                    new Vector2(external.xMax, external.yMax),
                    new Vector2(external.xMin, external.yMax),
                },
             },
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
}
