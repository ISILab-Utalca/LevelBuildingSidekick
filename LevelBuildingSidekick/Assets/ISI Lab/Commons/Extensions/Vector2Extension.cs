using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{

    public static Vector2Int ToInt(this Vector2 vector)
    {
        return new Vector2Int((int)vector.x, (int)vector.y);
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

}

public enum Orientation
{
    COLINEAR,
    CLOCKWISE,
    ANTICLOCKWISE
}
