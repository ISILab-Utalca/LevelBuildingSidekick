using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Utils
{
    public static class VectorExtension
    {
        public static float MinAbs(this Vector2 v)
        {
            return (Mathf.Abs(v.x) < Mathf.Abs(v.y)) ? v.x : v.y;
        }

        public static float MaxAbs(this Vector2 v)
        {
            return (Mathf.Abs(v.x) > Mathf.Abs(v.y)) ? v.x : v.y;
        }

        public static Vector2Int ToInt(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }
    }
}
