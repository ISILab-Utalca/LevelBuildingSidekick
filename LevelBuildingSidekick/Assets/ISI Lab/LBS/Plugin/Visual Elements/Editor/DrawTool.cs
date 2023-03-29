using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements; 

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

        Vector2 iniPos = new Vector2(grid.xMin, grid.yMin); // coordinates of top-left corner
        Vector2 endPos = new Vector2(grid.xMax, grid.yMax); // coordinates of bottom-right corner

        DrawSquareGrid(new List<List<Vector2>>()
        { 
            new List<Vector2>() 
            {
                    new Vector2(grid.xMin, grid.yMin),
                    new Vector2(grid.xMax, grid.yMin),
                    new Vector2(grid.xMax, grid.yMax),
                    new Vector2(grid.xMin, grid.yMax),
            },
        }
        , new Color(1, 0, 0, 0.45f)
        , new Color(0, 0, 0, 1)
        , 3
        , 1
        , 0.3f
        , true);

        //DrawLine(new Vector2(grid.xMin,grid.yMin), new Vector2(grid.xMax, grid.yMax), new Color(1, 0, 0, 0.05f), new Color(1, 0, 0, 1f), true, 3);
        //DrawTriangle(new Vector2(0,0), Color.blue, Color.black, 0.5f, 0, 50);
        //DrawPolygons(new Vector2(0, 0), 16, Color.blue, Color.black, 0.5f, 0 * Mathf.Deg2Rad, 50);
    }

    //Rotate a vector
    private Vector2 RotateVector2(Vector2 vec, float angle)
    {
        float x = vec.x;
        float y = vec.y;
        vec.x = x * Mathf.Cos(angle) - y * Mathf.Sin(angle);
        vec.y = x * Mathf.Sin(angle) + y * Mathf.Cos(angle);
        return vec;
    }
    //Draw a square with a hole inside
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
    //Draw a circle
    private void DrawCircle(Vector2 pos, float radius, Color color)
    {
        paint2D.fillColor = color;
        paint2D.BeginPath();
        paint2D.Arc(pos, radius, 0.0f, 360.0f);
        paint2D.Fill();
    }
    //Draw a line
    private void DrawLine(Vector2 iniPos, Vector2 endPos, Color color, Color border, bool dottedLine, float stroke = 3)
    {
        //Dotted Line
        if (dottedLine)
        {
            //Number of dots
            //if the number is odd | number of dots = (numDots-1)/2 
            //if the number is even | number of dots = (numDots/2) - 1
            //Prefer even number, because the generation dots gonna be better
            int numDots = 12;
            List<Vector2> dots = new List<Vector2>();


            // Calculate the position of the dots on the line
            for (int i = 0; i < numDots; i++)
            {
                Vector2 dotPosition = Vector2.Lerp(iniPos, endPos, (float)i / (numDots - 1));
                dots.Add(dotPosition);
            }

            for (int i = 1; i < dots.Count; i++)
            {
                //if the number is even, draw from the previous point to the actual
                if (i % 2 == 0)
                {
                    DrawLineWithList(new List<Vector2>() { dots[i - 1], dots[i] }, color, border, 3);
                }
                //if the number is odd, just doesn't draw the line, and leave a space
                else
                {

                }
            }
        }
        //If 'dottedLine' is false, just make a straight line
        else
        {
            DrawLineWithList(new List<Vector2>() { iniPos, endPos }, color, border, 3);
        }
    }
    //Draw a line with List<Vector2> instead of just two points
    private void DrawLineWithList(List<Vector2> point, Color color, Color border, float stroke = 1)
    {
        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        paint2D.MoveTo(point[0]);
        for (int i = 1; i < point.Count; i++)
        {
            paint2D.LineTo(point[i]);
        }

        paint2D.lineWidth = stroke;
        paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();
    }
    //Draw a square with differents points
    //in the example, just gave initial point and end point to make the entire square
    private void DrawSquareGrid(List<List<Vector2>> points, Color color, Color border, float stroke, float scale, float angle, bool dottedLine)
    {

        paint2D.strokeColor = new Color(0, 0, 0, 0);
        paint2D.fillColor = color;
        paint2D.BeginPath();

        if (dottedLine)
        {
            for (int j = 0; j < points.Count; j++)
            {
                for (int i = 0; i < points[j].Count; i++)
                {
                    Vector2 newVector2 = new Vector2();
                    newVector2.x = points[j][i].x * scale;
                    newVector2.y = points[j][i].y * scale;
                    float x = newVector2.x;
                    float y = newVector2.y;
                    newVector2.x = x * Mathf.Cos(angle) - y * Mathf.Sin(angle);
                    newVector2.y = x * Mathf.Sin(angle) + y * Mathf.Cos(angle);
                    points[j][i] = newVector2;
                }
            }
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

            // Calculate the size of the square
            float width = Mathf.Abs(endPos.x - initialPos.x);
            float height = Mathf.Abs(endPos.y - initialPos.y);
            float size = Mathf.Max(width, height);

            // Calculate the center of the square
            Vector2 center = new Vector2((initialPos.x + endPos.x) / 2, (initialPos.y + endPos.y) / 2);

            float halfSize = size / 2;
            Vector2 topLeft = new Vector2(center.x - halfSize, center.y + halfSize);
            Vector2 topRight = new Vector2(center.x + halfSize, center.y + halfSize);
            Vector2 bottomLeft = new Vector2(center.x - halfSize, center.y - halfSize);
            Vector2 bottomRight = new Vector2(center.x + halfSize, center.y - halfSize);

            Vector2 topLeftRotated = RotateVector2(topLeft, angle);
            Vector2 topRightRotated = RotateVector2(topRight, angle);
            Vector2 bottomLeftRotated = RotateVector2(bottomLeft, angle);
            Vector2 bottomRightRotated = RotateVector2(bottomRight, angle);

            // Draw the top line
            DrawLine(topLeftRotated, topRightRotated, color, border, dottedLine, stroke);

            // Draw the right line
            DrawLine(topRightRotated, bottomRightRotated, color, border, dottedLine, stroke);

            // Draw the bottom line
            DrawLine(bottomRightRotated, bottomLeftRotated, color, border, dottedLine, stroke);

            // Draw the left line
            DrawLine(bottomLeftRotated, topLeftRotated, color, border, dottedLine, stroke);

        }
        else
        {
            paint2D.strokeColor = border;
            paint2D.fillColor = color;
            paint2D.BeginPath();

            for (int j = 0; j < points.Count; j++)
            {
                for (int i = 0; i < points[j].Count; i++)
                {
                    Vector2 newVector2 = new Vector2();
                    newVector2.x = points[j][i].x * scale;
                    newVector2.y = points[j][i].y * scale;
                    float x = newVector2.x;
                    float y = newVector2.y;
                    newVector2.x = x * Mathf.Cos(angle) - y * Mathf.Sin(angle);
                    newVector2.y = x * Mathf.Sin(angle) + y * Mathf.Cos(angle);
                    points[j][i] = newVector2;
                }
            }
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

    //Draw a triangle
    private void DrawTriangle(Vector2 point, Color color, Color border, float stroke,float angle, float radius)
    {
        float angle1 = 0;
        float angle2 = 120;
        float angle3 = 240;

        List<Vector2> trianglePoints = new List<Vector2>()
        {
            new Vector2(point.x + radius * Mathf.Cos(angle1 * Mathf.Deg2Rad), point.y + radius * Mathf.Sin(angle1 * Mathf.Deg2Rad)),
            new Vector2(point.x + radius * Mathf.Cos(angle2 * Mathf.Deg2Rad), point.y + radius * Mathf.Sin(angle2 * Mathf.Deg2Rad)),
            new Vector2(point.x + radius * Mathf.Cos(angle3 * Mathf.Deg2Rad), point.y + radius * Mathf.Sin(angle3 * Mathf.Deg2Rad)),
        };

        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        for (int i = 0; i < trianglePoints.Count; i++)
        {
            Vector2 newVector2 = new Vector2();
            newVector2.x = trianglePoints[i].x;
            newVector2.y = trianglePoints[i].y;
            float x = newVector2.x;
            float y = newVector2.y;
            newVector2.x = x * Mathf.Cos(angle * Mathf.Deg2Rad) - y * Mathf.Sin(angle * Mathf.Deg2Rad);
            newVector2.y = x * Mathf.Sin(angle * Mathf.Deg2Rad) + y * Mathf.Cos(angle * Mathf.Deg2Rad);
            trianglePoints[i] = newVector2;
        }
        paint2D.MoveTo(trianglePoints[0]);
        for (int i = 1; i < trianglePoints.Count; i++)
        {
            paint2D.LineTo(trianglePoints[i]);
        }

        paint2D.ClosePath();
        paint2D.lineWidth = stroke;
        paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();
    }
    //Draw any polygon (in a range)
    private void DrawPolygons(Vector2 point, int edges, Color color, Color border, float stroke,float angle, float radius)
    {
        List<int> edgesDegree = new List<int>();
        List<Vector2> points = new List<Vector2>();


        if (edges <= 0)
        {
            DrawCircle(point, radius, color);
            return;
        }

        for (int i = 0; i < edges; i++)
        {
            edgesDegree.Add((360 / edges) * i);
            Vector2 newVector2 = new Vector2(point.x + radius * Mathf.Cos(edgesDegree[i] * Mathf.Deg2Rad), point.y + radius * Mathf.Sin(edgesDegree[i] * Mathf.Deg2Rad));
            points.Add(newVector2);
        }

        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 newVector2 = new Vector2();
            newVector2.x = points[i].x;
            newVector2.y = points[i].y;
            float x = newVector2.x;
            float y = newVector2.y;
            newVector2.x = x * Mathf.Cos(angle * Mathf.Deg2Rad) - y * Mathf.Sin(angle * Mathf.Deg2Rad);
            newVector2.y = x * Mathf.Sin(angle * Mathf.Deg2Rad) + y * Mathf.Cos(angle * Mathf.Deg2Rad);
            points[i] = newVector2;
        }
        paint2D.MoveTo(points[0]);
        for (int i = 1; i < points.Count; i++)
        {
            paint2D.LineTo(points[i]);
        }

        paint2D.ClosePath();
        paint2D.lineWidth = stroke;
        paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();
    }
}
