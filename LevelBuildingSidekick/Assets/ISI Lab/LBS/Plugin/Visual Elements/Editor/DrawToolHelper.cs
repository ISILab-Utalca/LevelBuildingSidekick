using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

public static class DrawToolHelper
{

    //Draw a square with a hole inside
    public static void DrawHoleyShape(this Painter2D paint2D, List<List<Vector2>> points, Color color, Color border, float stroke = 1)
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

    // Draw a Box
    public static void DrawSelectionBox(this Painter2D paint2D, Vector2 pos1, Vector2 pos2, Color color, int width = 1)
    {
        var fillColor = color * new Color(1, 1, 1, 0.2f);
        
        var points = new List<Vector2>() 
        { 
            new Vector2(pos1.x, pos1.y),
            new Vector2(pos2.x, pos1.y),
            new Vector2(pos1.x, pos2.y),
            new Vector2(pos2.x, pos2.y),
        };
        paint2D.DrawLine(points,color,fillColor,4);
    }

    //Draw a circle
    public static void DrawCircle(this Painter2D paint2D, Vector2 pos, float radius, Color color)
    {
        
        paint2D.fillColor = color;
        paint2D.BeginPath();
        paint2D.Arc(pos, radius, 0.0f, 360.0f);
        paint2D.Fill();
    }

    //Draw a line
    public static void DrawLine(this Painter2D paint2D, Vector2 iniPos, Vector2 endPos, Color color, Color border, bool dottedLine, float stroke = 3,int divs = 12)
    {
        //Dotted Line
        if (dottedLine)
        {
            List<Vector2> dots = new List<Vector2>();

            for (int i = 0; i < divs; i++)
            {
                Vector2 dotPosition = Vector2.Lerp(iniPos, endPos, (float)i / (divs - 1));
                dots.Add(dotPosition);
            }

            for (int i = 1; i < dots.Count; i++)
            {
                if (i % 2 == 0)
                {
                    DrawLine( paint2D,new List<Vector2>() { dots[i - 1], dots[i] }, color, border, 3);
                }
                else
                { 
                }
            }
        }
        //If 'dottedLine' is false, just make a straight line
        else
        {
            DrawLine( paint2D,new List<Vector2>() { iniPos, endPos }, color, border, 3);
        }
    }
    //Draw a line with List<Vector2> instead of just two points
    public static void DrawLine(this Painter2D paint2D, List<Vector2> point, Color color, Color border, float stroke = 1)
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
    public static void DrawSquareGrid(this Painter2D paint2D, List<List<Vector2>> points, Color color, Color border, float stroke, float scale, float angle)
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

    [System.Obsolete]
    public static void DrawTriangle(this Painter2D paint2D, Vector2 point, Color color, Color border, float stroke, float angle, float radius)
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
    public static void DrawPolygons(this Painter2D paint2D, Vector2 point, int edges, Color color, Color border, float stroke, float angle, float radius)
    {
        List<Vector2> points = new List<Vector2>();

        if (edges <= 0)
        {
            DrawCircle(paint2D, point, radius, color);
            return;
        }

        for (int i = 0; i < edges; i++)
        {
            var _angle = (360f / edges * 1f) * i;
            Vector2 vec = new Vector2( radius * Mathf.Cos(_angle * Mathf.Deg2Rad), radius * Mathf.Sin(_angle * Mathf.Deg2Rad));
            points.Add(vec);
        }

        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 vec = new Vector2();
            vec.x = points[i].x;
            vec.y = points[i].y;
            float x = vec.x;
            float y = vec.y;
            vec.x = (x * Mathf.Cos(angle * Mathf.Deg2Rad) - y * Mathf.Sin(angle * Mathf.Deg2Rad));
            vec.y = (x * Mathf.Sin(angle * Mathf.Deg2Rad) + y * Mathf.Cos(angle * Mathf.Deg2Rad));
            points[i] = vec + point;
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

    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = vector.x;
        float ty = vector.y;
        vector.x = (cos * tx) - (sin * ty);
        vector.y = (sin * tx) + (cos * ty);
        return vector;
    }
}
