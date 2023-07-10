using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{

    public static Vector2Int ToInt(this Vector2 vector)
    {
        return new Vector2Int((int)vector.x, (int)vector.y);
    }

    public static List<Vector2> AsComponents(this Vector2 vector)
    {
        return new List<Vector2>() { new Vector2(vector.x, 0), new Vector2(0, vector.y) };
    }

    public static List<Vector2Int> AsComponents(this Vector2Int vector)
    {
        return new List<Vector2Int>() { new Vector2Int(vector.x, 0), new Vector2Int(0, vector.y) };
    }

    public static List<Vector2Int> Get4Connected(this Vector2Int vector)
    {
        var toR = new List<Vector2Int>();
        toR.Add(new Vector2Int(vector.x - 1, vector.y));
        toR.Add(new Vector2Int(vector.x + 1, vector.y));
        toR.Add(new Vector2Int(vector.x, vector.y - 1));
        toR.Add(new Vector2Int(vector.x, vector.y + 1));
        return toR;
    }

    public static List<Vector2Int> Get8Connected(this Vector2Int vector)
    {
        var toR = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                toR.Add(new Vector2Int(vector.x + i, vector.y + j));
            }
        }
        return toR;
    }

    public static bool IsInLine(this Vector2 vector, Vector2 start, Vector2 end)
    {
        if (start.x <= Mathf.Max(end.x, vector.x)
        && start.x <= Mathf.Min(end.x, vector.x)
        && (start.y <= Mathf.Max(end.y, vector.y)
        && start.y <= Mathf.Min(end.y, vector.y)))
            return true;

        return false;
    }

    public static Orientation IntersectionOrientation(this Vector2 point, Vector2 start,  Vector2 end)
    {
        float val = (point.y - start.y) * (end.x - point.x)
      - (point.x - start.x) * (end.y - point.y);

        if (val == 0)

            // Colinear
            return Orientation.COLINEAR;

        else if (val < 0)

            // Anti-clockwise direction
            return Orientation.ANTICLOCKWISE;

        // Clockwise direction
        return Orientation.CLOCKWISE;
    }

    public static bool LinesIntersect(this Vector2 start, Vector2 end, Vector2 point1, Vector2 point2)
    {
        Orientation o1 = start.IntersectionOrientation(end, point1);
        Orientation o2 = start.IntersectionOrientation(end, point2);
        Orientation o3 = point1.IntersectionOrientation(point2, start);
        Orientation o4 = point1.IntersectionOrientation(point2, end);

        if (o1 != o2 && o3 != o4)
            return true;

        if(o1 == Orientation.COLINEAR && point1.IsInLine(start, end))
            return true;

        if (o2 == Orientation.COLINEAR && point2.IsInLine(start, end))
            return true;

        if (o3 == Orientation.COLINEAR && start.IsInLine(point1, point2))
            return true;

        if (o4 == Orientation.COLINEAR && end.IsInLine(point1, point2))
            return true;

        return false;
    }

    public static Vector2Int Multiply(this Vector2Int vec, float value)
    {
        vec.x = (int)(vec.x * value);
        vec.y = (int)(vec.y * value);
        return vec;
    }

    public static Vector2Int Divided(this Vector2Int vec, float value)
    {
        vec.x = (int)(vec.x / value);
        vec.y = (int)(vec.y / value);
        return vec;
    }

    public static float DistanceToLine(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {

        float dist = Vector2.Distance(lineStart, lineEnd);
        if (dist == 0)
        {
            return Vector2.Distance(point, lineStart);
        }


        float perc = Vector2.Dot((point - lineStart) / dist, (lineEnd - lineStart) / dist);

        Vector2 v = lineStart + ((lineEnd - lineStart) * perc);

        return Vector2.Distance(v, point);
    }

    public static Vector2 Divided(this Vector2 vec, float value)
    {
        vec.x = (int)(vec.x / value);
        vec.y = (int)(vec.y / value);
        return vec;
    }

    public static float Distance(this Vector2 vec, DistanceType distType)
    {

        var dist = 2f;
        switch (distType)
        {
            case DistanceType.CONNECT_4: dist = 2f; break;
            //case DistanceType.EUCLIDEAN: dist = 1.4f; break;
            case DistanceType.CONNECT_8: dist = 1f; break;
        }

        var min = Mathf.Abs(vec.x) < Mathf.Abs(vec.y) ? Mathf.Abs(vec.x) : Mathf.Abs(vec.y);
        var max = Mathf.Abs(vec.x) > Mathf.Abs(vec.y) ? Mathf.Abs(vec.x) : Mathf.Abs(vec.y);

        return (max - min) + min * dist;
    }
}

public enum Orientation
{
    COLINEAR,
    CLOCKWISE,
    ANTICLOCKWISE
}

public enum DistanceType
{
    CONNECT_4,
    CONNECT_8
}
