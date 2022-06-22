using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class Math
    {
        public static float PointToLineDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            float dist = Vector2.Distance(lineStart, lineEnd);
            if (dist == 0)
            {
                return Vector2.Distance(point, lineStart);
            }

            dist = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(lineStart - point, lineEnd - lineStart) / dist));

            Vector2 v = lineStart + dist * (lineEnd - lineStart);

            return Vector2.Distance(point, v);
        }
    }
}

