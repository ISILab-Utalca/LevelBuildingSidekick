using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;

public class DrawTool : GraphElement
{
    public new class UxmlFactory : UxmlFactory<DrawTool, UxmlTraits> { }

    private Painter2D paint2D;
    private Vector2 initialPos, endPos;
    private Rect grid;

    public DrawTool()
    {
        this.style.width = 50; // (!) deshardcodear
        this.style.height = 50;

        this.initialPos = new Vector2(0,0);
        this.endPos = new Vector2(50,50);
        this.grid = new Rect(initialPos, endPos);

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

       /*
        DrawSquareGrid(new List<List<Vector2>>()
             {
                new List<Vector2>() {
                    new Vector2(grid.xMin, grid.yMin),
                    new Vector2(grid.xMax, grid.yMin),
                    new Vector2(grid.xMax, grid.yMax),
                    new Vector2(grid.xMin, grid.yMax),
                },
             },
        new Color(1, 0, 0, 0.05f),
        new Color(1, 0, 0, 1f),
        3
        );
       */
        DrawLine
        (
            new Vector2(grid.xMin,grid.yMin), new Vector2(grid.xMax, grid.yMax),
            new Color(1, 0, 0, 0.05f),
            new Color(1, 0, 0, 1f),
            true,
            3
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

    //Draw a line
    private void DrawLine(Vector2 iniPos, Vector2 endPos, Color color, Color border, bool dotedLine, float stroke = 3)
    {
        //Dot Line ArrowShape
        if (dotedLine)
        {
            int numDots = 10;
            float dotSize = 0.1f;
            List<Vector2> dots = new List<Vector2>();

            //Colors
            paint2D.strokeColor = border;
            paint2D.fillColor = color;
            paint2D.BeginPath();

            // Calculate the position of the dots on the line
            for (int i = 0; i < numDots; i++)
            {
                Vector2 dotPosition = Vector2.Lerp(iniPos, endPos, (float)i / (numDots - 1));
                dots.Add(dotPosition);
            }

            // Draw the dots as rectangles
            for (int i = 0; i < dots.Count; i++)
            {
                Vector2 dot = dots[i];
                Vector2 rectTopLeft = dot - new Vector2(dotSize / 2, dotSize / 2);
                Vector2 rectBottomRight = dot + new Vector2(dotSize / 2, dotSize / 2);

                paint2D.MoveTo(new Vector2(rectTopLeft.x, rectTopLeft.y));
                paint2D.LineTo(new Vector2(rectBottomRight.x, rectTopLeft.y));
                paint2D.LineTo(new Vector2(rectBottomRight.x, rectBottomRight.y));
                paint2D.LineTo(new Vector2(rectTopLeft.x, rectBottomRight.y));
                paint2D.LineTo(new Vector2(rectTopLeft.x, rectTopLeft.y));
            }
            paint2D.lineWidth = stroke;
            paint2D.Fill(FillRule.OddEven);
            paint2D.Stroke();
        }
        //Dot Line SquareShape
        /*
        if (dotedLine)
        {
            int numDots = 3;
            float dotSize = 0.1f;
            List<Vector2> dots = new List<Vector2>();

            //Colors
            paint2D.strokeColor = border;
            paint2D.fillColor = color;
            paint2D.BeginPath();

            // Calculate the position of the dots on the line
            for (int i = 0; i < numDots; i++)
            {
                Vector2 dotPosition = Vector2.Lerp(iniPos, endPos, (float)i / (numDots - 1));
                dots.Add(dotPosition);
            }

            // Draw the dots as rectangles
            for (int i = 0; i < dots.Count; i++)
            {
                Vector2 dot = dots[i];
                Vector2 rectTopLeft = dot - new Vector2(dotSize / 2, dotSize / 2);
                Vector2 rectBottomRight = dot + new Vector2(dotSize / 2, dotSize / 2);
                Vector2 rectTopRight = new Vector2(rectBottomRight.x, rectTopLeft.y);
                Vector2 rectBottomLeft = new Vector2(rectTopLeft.x, rectBottomRight.y);

                paint2D.MoveTo(rectTopLeft);
                paint2D.LineTo(rectTopRight);
                paint2D.LineTo(rectBottomRight);
                paint2D.LineTo(rectBottomLeft);
                paint2D.LineTo(rectTopLeft);
                paint2D.LineTo(rectTopRight);

            }

            paint2D.lineWidth = stroke;
            paint2D.Fill(FillRule.OddEven);
            paint2D.Stroke();
        }
        */
        else
        {
            //Colors
            paint2D.strokeColor = border;
            paint2D.fillColor = color;
            paint2D.BeginPath();

            // Move to the initial position
            paint2D.MoveTo(iniPos);

            // Draw the line
            paint2D.LineTo(endPos);

            // Close the path
            paint2D.ClosePath();

            // Set the stroke width
            paint2D.lineWidth = stroke;

            // Fill the line
            paint2D.Fill(FillRule.OddEven);

            // Stroke the line
            paint2D.Stroke();
        }
    }

    //Draw a square
    private void DrawSquareGrid(List<List<Vector2>> points, Color color, Color border, float stroke)
    {
        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        foreach (var path in points)
        {
            paint2D.MoveTo(path[0]);
            for(int i = 1; i < path.Count; i++)
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
