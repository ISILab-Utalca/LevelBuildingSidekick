using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

public static class Painter2DExtensions
{
    /// <summary>
    /// Draws a shape with holes on a 2D canvas using a list of paths defined by a set of points.
    /// </summary>
    /// <param name="paint2D">The 2D canvas to draw on.</param>
    /// <param name="points">A list of paths defined by a set of points.</param>
    /// <param name="color">The fill color of the shape.</param>
    /// <param name="border">The stroke color of the shape.</param>
    /// <param name="stroke">The width of the stroke. Default value is 1.</param>
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

    /// <summary>
    /// Draws a selection box on a 2D canvas with the given position vectors and color.
    /// </summary>
    /// <param name="paint2D">The 2D canvas to draw on.</param>
    /// <param name="pos1">The starting position vector of the selection box.</param>
    /// <param name="pos2">The ending position vector of the selection box.</param>
    /// <param name="color">The color of the selection box.</param>
    /// <param name="width">The width of the selection box border. Default value is 1.</param>
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
        paint2D.DrawPolygon(points, color, fillColor, 4);
    }

    /// <summary>
    /// Draws a circle on a 2D canvas with the given position vector, radius and color.
    /// </summary>
    /// <param name="paint2D">The 2D canvas to draw on.</param>
    /// <param name="pos">The position vector of the center of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public static void DrawCircle(this Painter2D paint2D, Vector2 pos, float radius, Color color)
    {
        paint2D.fillColor = color;
        paint2D.BeginPath();
        paint2D.Arc(pos, radius, 0.0f, 360.0f);
        paint2D.Fill();
    }

    public static void DrawDottedBox(this Painter2D painter2D, Vector2 pos1, Vector2 pos2, Color color, int width = 1)
    {
        float lineWidth = width;

        painter2D.DrawDottedLine(pos1, new Vector2(pos2.x, pos1.y), color, lineWidth);
        painter2D.DrawDottedLine(new Vector2(pos2.x, pos1.y), pos2, color, lineWidth);
        painter2D.DrawDottedLine(pos2, new Vector2(pos1.x, pos2.y), color, lineWidth);
        painter2D.DrawDottedLine(new Vector2(pos1.x, pos2.y), pos1, color, lineWidth);
    }

    /// <summary>
    /// Draws a dotted line on a 2D canvas between two given positions with the given color, stroke and line width.
    /// </summary>
    /// <param name="paint2D">The 2D canvas to draw on.</param>
    /// <param name="initPos">The starting position of the dotted line.</param>
    /// <param name="endPos">The ending position of the dotted line.</param>
    /// <param name="color">The color of the dotted line.</param>
    /// <param name="stroke">The distance between two consecutive dots in the dotted line.</param>
    /// <param name="lineWidth">The width of the line.</param>
    public static void DrawDottedLine(this Painter2D paint2D, Vector2 initPos, Vector2 endPos, Color color, float stroke = 5, float lineWidth = 15)
    {
        var dir = (endPos - initPos).normalized;
        var distance = Vector2.Distance(initPos, endPos);
        var divs = distance / lineWidth;

        List<Vector2> dots = new List<Vector2>();

        for (int i = 0; i < divs; i++)
        {
            Vector2 dotPosition = Vector2.Lerp(initPos, endPos, (float)i / (divs - 1));
            dots.Add(dotPosition);
        }

        for (int i = 1; i < dots.Count; i++)
        {
            if (i % 2 == 0)
            {
                DrawLine(paint2D, dots[i - 1], dots[i], color, stroke);
            }
        }
    }

    /// <summary>
    /// Draw a line with given points, color and stroke.
    /// </summary>
    /// <param name="paint2D"></param>
    /// <param name="iniPos"></param>
    /// <param name="endPos"></param>
    /// <param name="color"></param>
    /// <param name="stroke"></param>
    public static void DrawLine(this Painter2D paint2D, Vector2 iniPos, Vector2 endPos, Color color, float stroke = 5)
    {
        DrawPolygon(paint2D, new List<Vector2>() { iniPos, endPos }, new Color(0, 0, 0, 0), color, stroke);
    }

    public static void DrawDottedPolygon(this Painter2D paint2D, List<Vector2> point, Color color, float stroke = 1, bool closed = false)
    {
        for (int i = 1; i < point.Count; i++)
        {
            paint2D.DrawDottedLine(point[i - 1], point[i], color, stroke);
        }

        if (closed)
        {
            paint2D.DrawDottedLine(point[point.Count - 1], point[0], color, stroke);
        }
    }

    /// <summary>
    /// Draws a polygon shape with given points, color, and border. Allows setting stroke width and whether the polygon is closed or not.
    /// </summary>
    /// <param name="paint2D">The 2D painter object to use for drawing.</param>
    /// <param name="point">The list of points that define the polygon shape.</param>
    /// <param name="color">The fill color of the polygon.</param>
    /// <param name="border">The border color of the polygon.</param>
    /// <param name="stroke">The width of the polygon border stroke. Defaults to 1.</param>
    /// <param name="closed">Whether the polygon shape should be closed or not. Defaults to false.</param>
    public static void DrawPolygon(this Painter2D paint2D, List<Vector2> point, Color color, Color border, float stroke = 1, bool closed = false)
    {
        paint2D.strokeColor = border;
        paint2D.fillColor = color;
        paint2D.BeginPath();

        paint2D.MoveTo(point[0]);
        for (int i = 1; i < point.Count; i++)
        {
            paint2D.LineTo(point[i]);
        }

        if(closed)
            paint2D.ClosePath();

        paint2D.lineWidth = stroke;
        paint2D.Fill(FillRule.OddEven);
        paint2D.Stroke();
    }

    /// <summary>
    /// Draws a grid of squares with given points, color, and border. Allows setting stroke width, scale, and rotation angle.
    /// </summary>
    /// <param name="paint2D">The 2D painter object to use for drawing.</param>
    /// <param name="points">A list of lists of Vector2 points, where each inner list represents a row of squares.</param>
    /// <param name="color">The fill color of the squares.</param>
    /// <param name="border">The border color of the squares.</param>
    /// <param name="stroke">The width of the square border stroke.</param>
    /// <param name="scale">The scale factor to apply to each square.</param>
    /// <param name="angle">The rotation angle, in degrees, to apply to the grid.</param>
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

    /// <summary>
    /// Rotates a Vector2 object by a given number of degrees around the origin point (0,0).
    /// </summary>
    /// <param name="vector">The Vector2 object to rotate.</param>
    /// <param name="degrees">The number of degrees to rotate by.</param>
    /// <returns>The rotated Vector2 object.</returns>
    private static Vector2 Rotate(this Vector2 vector, float degrees)
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
