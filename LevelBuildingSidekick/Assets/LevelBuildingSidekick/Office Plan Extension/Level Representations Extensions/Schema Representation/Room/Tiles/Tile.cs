using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Schema
{
    [System.Serializable]
    public class Tile
    {
        public int x;
        public int y;
        public int sideCode { get; private set; }
        public int diagCode { get; private set; }

        public Tile(int _x, int _y)
        {
            x = _x;
            y = _y;
            sideCode = 15;
            diagCode = 15;
        }

        public Tile(Vector2Int v)
        {
            x = v.x;
            y = v.y;
            sideCode = 15;
            diagCode = 15;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() * 31 + ((y.GetHashCode() << 16) | (y.GetHashCode() >> (32 - 16)));
        }

        public void SetSides(int n)
        {
            sideCode = n;
        }

        public void SetDiagonals(int n)
        {
            diagCode = n;
        }

        public Tile Copy()
        {
            return new Tile(x, y);
        }

        #region Operator +
        public static Tile operator +(Tile a, Tile b) => new Tile(a.x + b.x, a.y + b.y);
        public static Tile operator +(Tile a, Vector2 b) => new Tile(a.x + (int)b.x, a.y + (int)b.y);
        public static Vector2 operator +(Vector2 a, Tile b) => new Vector2(a.x + b.x, a.y + b.y);
        #endregion

        #region Operator -
        public static Tile operator -(Tile a, Tile b) => new Tile(a.x - b.x, a.y - b.y);
        public static Tile operator -(Tile a, Vector2 b) => new Tile(a.x - (int)b.x, a.y - (int)b.y);
        public static Vector2 operator -(Vector2 a, Tile b) => new Vector2(a.x - b.x, a.y - b.y);
        #endregion

        #region Operator ==
        public static bool operator ==(Tile a, Tile b) => a.x == b.x && a.y == b.y;
        public static bool operator ==(Tile a, Vector2 b) => a.x == b.x && a.y == b.y;
        public static bool operator ==(Vector2 a, Tile b) => a.x == b.x && a.y == b.y;
        #endregion

        #region Operator !=
        public static bool operator !=(Tile a, Tile b) => a.x != b.x || a.y != b.y;
        public static bool operator !=(Tile a, Vector2 b) => a.x != b.x || a.y != b.y;
        public static bool operator !=(Vector2 a, Tile b) => a.x != b.x || a.y != b.y;
        #endregion

        public static Tile zero => new Tile(0, 0);
        public Vector2Int vector => new Vector2Int(x, y);

        public override bool Equals(object obj)
        {
            if (obj is Tile)
            {
                var t = obj as Tile;
                return t.x == x && t.y == y;
            }
            return false;
        }

    }
}
