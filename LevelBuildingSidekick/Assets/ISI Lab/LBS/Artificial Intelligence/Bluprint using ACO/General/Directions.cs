using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Optimization.Utils
{

    public static class Directions
    {
        public static List<Vector2Int> directions_8 = new List<Vector2Int>()
    {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(1, 1),  // Right-Up
        new Vector2Int(0, 1),  // Up
        new Vector2Int(-1, 1), // Left-Up
        new Vector2Int(-1, 0), // Left
        new Vector2Int(-1, -1),// Left-Down
        new Vector2Int(0, -1), // Down
        new Vector2Int(1, -1)  // Right-Down
    };

        public static List<Vector2Int> directions_4 = new List<Vector2Int>()
    {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, 1),  // Up
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, -1)  // Down
    };

        public static List<Directions.Dirs_4> dirs_4 = new List<Directions.Dirs_4>()
    {
        Dirs_4.Right,
        Dirs_4.Up,
        Dirs_4.Left,
        Dirs_4.Down
    };

        public enum Dirs_8
        {
            Right = 0,
            RightUp = 1,
            Up = 2,
            LeftUp = 3,
            Left = 4,
            LeftDown = 5,
            Down = 6,
            RightDown = 7,
            None = -1
        }

        public enum Dirs_4
        {
            Right = 0,
            Up = 1,
            Left = 2,
            Down = 3,
            None = -1
        }

        public static Dirs_4 Next(this Dirs_4 dir)
        {
            switch (dir)
            {
                case Dirs_4.Right:
                    return Dirs_4.Up;
                case Dirs_4.Up:
                    return Dirs_4.Left;
                case Dirs_4.Left:
                    return Dirs_4.Down;
                case Dirs_4.Down:
                    return Dirs_4.Right;
                default:
                    return Dirs_4.None;
            }
        }

        /// <summary>
        /// Get the directions of a vector.
        /// </summary>
        /// <param name="dir">The vector to analyze.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description><b>First</b> goes the horizontal directions.</description></item>
        /// <item><description><b>Second</b> goes the vertical directions.</description></item>
        /// <item><description><b>If</b> the vector is <b>zero</b>, returns none.</description></item>
        /// </list>
        /// </returns>
        public static List<Dirs_4> _4_Dirs(Vector2 dir)
        {
            var toR = new List<Dirs_4>();
            if (dir.x > 0)
            {
                toR.Add(Dirs_4.Right);
            }
            else if (dir.x < 0)
            {
                toR.Add(Dirs_4.Left);
            }
            else
            {
                toR.Add(Dirs_4.None);
            }

            if (dir.y > 0)
            {
                toR.Add(Dirs_4.Up);
            }
            else if (dir.y < 0)
            {
                toR.Add(Dirs_4.Down);
            }
            else
            {
                toR.Add(Dirs_4.None);
            }

            return toR;
        }

        public static List<Dirs_4> AllDirs()
        {
            return Directions.dirs_4;
        }

        public static List<Dirs_4> AngulatedDirs(Vector2 dir)
        {
            var toR = new List<Dirs_4>();
            var angle = Vector2.SignedAngle(Vector2.right, dir);
            //var toR = new List<Dirs_4>();
            if (angle > 0 && angle <= 22.5f)
            {
                toR.Add(Dirs_4.Right);
            }
            else if (angle > 22.5f && angle <= 67.5f)
            {
                toR.Add(Dirs_4.Right);
                toR.Add(Dirs_4.Up);
            }
            else if (angle > 67.5f && angle <= 112.5f)
            {
                toR.Add(Dirs_4.Up);
            }
            else if (angle > 112.5f && angle <= 157.5f)
            {
                toR.Add(Dirs_4.Up);
                toR.Add(Dirs_4.Left);
            }
            else if (angle > 157.5f && angle <= 180f)
            {
                toR.Add(Dirs_4.Left);
            }
            else if (angle < 0 && angle >= -22.5f)
            {
                toR.Add(Dirs_4.Right);
            }
            else if (angle < -22.5f && angle >= -67.5f)
            {
                toR.Add(Dirs_4.Right);
                toR.Add(Dirs_4.Down);
            }
            else if (angle < -67.5f && angle >= -112.5f)
            {
                toR.Add(Dirs_4.Down);
            }
            else if (angle < -112.5f && angle >= -157.5f)
            {
                toR.Add(Dirs_4.Down);
                toR.Add(Dirs_4.Left);
            }
            else if (angle < -157.5f && angle >= -180f)
            {
                toR.Add(Dirs_4.Left);
            }

            return toR;
        }

        public static List<Dirs_4> GetDirAxis(Vector2 dir)
        {
            var toR = new List<Dirs_4>();

            if (dir == Vector2.zero)
            {
                toR.Add(Dirs_4.None);
                return toR;
            }

            if (dir.x == 0)
            {
                toR.Add(Dirs_4.Up);
                toR.Add(Dirs_4.Down);
            }
            else if (dir.y == 0)
            {
                toR.Add(Dirs_4.Right);
                toR.Add(Dirs_4.Left);
            }
            else
            {
                Debug.Log("Este vector es diagonal, deberia ser ignorado");
                toR.Add(Dirs_4.None);
            }

            return toR;
        }

        public static List<Vector2Int> EnumsToVector(List<Dirs_4> dir)
        {
            var toR = new List<Vector2Int>();
            foreach (var d in dir)
            {
                if (d == Dirs_4.None)
                {
                    toR.Add(Vector2Int.zero);
                    continue;
                }

                var v = directions_4[(int)d];
                toR.Add(v);
            }

            return toR;
        }
    }
}