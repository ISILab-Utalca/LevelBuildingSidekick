using Optimization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Optimization.Data
{
    public class Tile : ICloneable
    {
        public Map owner;
        public int roomID = -1;

        #region Constructors
        public Tile(Map owner)
        {
            this.roomID = -1;
            this.owner = owner;
        }

        public object Clone() // TODO: Impelentar el sistema de diccionario que permite clonar referencias.
        {
            return new Tile(null)
            {
                roomID = this.roomID,
            };
        }
        #endregion
    }

    public class Map : ICloneable
    {
        #region Properties
        public RectInt Bounds
        {
            get
            {
                var min = new Vector2Int(int.MaxValue, int.MaxValue);
                var max = new Vector2Int(int.MinValue, int.MinValue);

                foreach (var r in rooms)
                {
                    foreach (var t in r.Value)
                    {
                        if (t.Key.x < min.x)
                            min.x = t.Key.x;
                        if (t.Key.x > max.x)
                            max.x = t.Key.x;

                        if (t.Key.y < min.y)
                            min.y = t.Key.y;
                        if (t.Key.y > max.y)
                            max.y = t.Key.y;
                    }

                }
                return new RectInt(min.x, min.y, max.x - min.x, max.y - min.y);
            }
        }

        public int Width
        {
            get
            {
                var min = int.MaxValue;
                var max = int.MinValue;

                foreach (var r in rooms)
                {
                    foreach (var t in r.Value)
                    {
                        if (t.Key.x < min)
                            min = t.Key.x;
                        if (t.Key.x > max)
                            max = t.Key.x;
                    }

                }
                return (max - min) + 1;
            }
        }

        public int Height
        {
            get
            {
                var min = int.MaxValue;
                var max = int.MinValue;

                foreach (var r in rooms)
                {
                    foreach (var t in r.Value)
                    {
                        if (t.Key.y < min)
                            min = t.Key.y;
                        if (t.Key.y > max)
                            max = t.Key.y;
                    }

                }
                return (max - min) + 1;
            }
        }

        public Vector2Int Center
        {
            get
            {
                var sum = Vector2Int.zero;
                var i = 0;
                foreach (var r in rooms)
                {
                    foreach (var t in r.Value)
                    {
                        sum.x += t.Key.x;
                        sum.y += t.Key.y;
                        i++;
                    }
                }
                return new Vector2Int(sum.x / i, sum.y / i); // BUG?: si no hay rooms, se divide por 0
            }
        }

        public int Area
        {
            get
            {
                var area = 0;
                foreach (var r in rooms)
                {
                    foreach (var t in r.Value)
                    {
                        area++;
                    }
                }
                return area;
            }
        }
        #endregion

        #region Variables
        public Dictionary<int, Dictionary<Vector2Int, Tile>> rooms = new();
        #endregion

        #region Constructors
        public Map()
        {

        }

        public object Clone()
        {
            var map = new Map();

            foreach (var r in this.rooms)
            {
                var room = new Dictionary<Vector2Int, Tile>();
                foreach (var t in r.Value)
                {
                    var nTile = t.Value.Clone() as Tile;
                    nTile.owner = map;
                    room.Add(t.Key, nTile);
                }
                map.rooms.Add(r.Key, room);
            }
            return map;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Set the room id to the tiles in the positions list,
        /// and recalculate the neigthbors and walls.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="roomID"></param>
        public void SetRoomTiles(List<Vector2Int> positions, int roomID)
        {
            // set room owner id
            for (int i = 0; i < positions.Count; i++)
            {
                var x = positions[i].x;
                var y = positions[i].y;

                Tile tile = null;
                foreach (var r in rooms)
                {
                    var pos = new Vector2Int(x, y);
                    if (r.Value.ContainsKey(pos))
                    {
                        tile = r.Value[pos];
                        r.Value.Remove(new Vector2Int(x, y));
                        break;
                    }
                }

                if (tile == null)
                {
                    tile = new Tile(this);
                    tile.roomID = roomID;

                    if (!rooms.ContainsKey(roomID))
                    {
                        rooms.Add(roomID, new Dictionary<Vector2Int, Tile>());
                    }
                    rooms[roomID].Add(new Vector2Int(x, y), tile);
                }
                else
                {
                    tile.roomID = roomID;
                    if (!rooms.ContainsKey(roomID))
                    {
                        rooms.Add(roomID, new Dictionary<Vector2Int, Tile>());
                    }
                    rooms[roomID].Add(new Vector2Int(x, y), tile);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of walls comprising a room's perimeter, 
        /// each represented by points and their facing direction.
        /// </summary>
        /// <param name="roomID">The room identifier.</param>
        /// <returns>A list of wall points and their facing direction.</returns>
        /*
        public List<(Vector2Int[], Vector2Int)> GetWalls(int roomID)
        {
            var r = rooms[roomID];
            var walls = new List<(Vector2Int[],Vector2Int)>();
            var horizontal = GetHorizontalWalls(roomID);
            var vertical = GetVerticalWalls(roomID);
            walls.AddRange(horizontal);
            walls.AddRange(vertical);
            return walls;
        }
        */

        public List<(Vector2Int[], Vector2Int)> GetWalls(int roomID)
        {
            var toR = new List<(Vector2Int[], Vector2Int)>();
            var candidates = new Dictionary<(Vector2Int, Directions.Dirs_4), Vector2Int>();

            var concave = GetConcaveCorners(roomID);
            var convex = GetConvexCorners(roomID);
            var corners = new List<Vector2Int>();
            corners.AddRange(concave);
            corners.AddRange(convex);

            for (int i = 0; i < corners.Count(); i++)
            {
                var p1 = corners[i];
                for (int j = 0; j < corners.Count(); j++)
                {
                    var p2 = corners[j];

                    // ignoramos a los pares de esquinas que no esten alineados
                    if (p1.x != p2.x && p1.y != p2.y)
                        continue;

                    var wDir = Directions.GetDirAxis(p1 - p2);

                    for (int k = 0; k < Directions.dirs_4.Count(); k++)
                    {
                        var currentDir = Directions.dirs_4[k];

                        // ignoramos las direciones que son paralelas a la pared
                        if (wDir.Contains(currentDir))
                            continue;

                        var tuple = (p1, currentDir);

                        if (!candidates.TryGetValue(tuple, out var other))
                        {
                            candidates.Add(tuple, p2);
                        }
                        else
                        {
                            if (other == p1)
                            {
                                candidates[tuple] = p2;
                            }
                            else if (p1 != p2)
                            {
                                var dist = Vector2Int.Distance(p1, p2);
                                var oDist = Vector2Int.Distance(p1, other);

                                // Nos quedamos con la muralla mas corta
                                if (dist < oDist)
                                {
                                    candidates[tuple] = p2;
                                }
                            }
                        }
                    }
                }
            }

            // Remueve muros repetidos
            for (int i = candidates.Count() - 1; i >= 0; i--)
            {
                var ((p1, d), p2) = candidates.ElementAt(i);
                if (candidates.TryGetValue((p2, d), out var o))
                {
                    if (o == p1 && p1 != p2)
                        candidates.Remove((p2, d));
                }
            }

            // Selecciona las murallas que no tengan tiles en frente
            foreach (var ((p1, d), p2) in candidates)
            {
                if (p1 == p2)
                {
                    toR.Add((new Vector2Int[] { p1 }, Directions.directions_4[(int)d]));
                    continue;
                }

                var points = GeneralUtils.GetPointsBetween(p1, p2);
                var include = true;
                for (int i = 1; i < points.Count() - 1; i++)
                {
                    var point = points[i];
                    var other = point + Directions.directions_4[(int)d];
                    if (rooms[roomID].ContainsKey(other))
                    {
                        include = false;
                        break;
                    }
                }

                if (include)
                {
                    toR.Add((points.ToArray(), Directions.directions_4[(int)d]));
                }
            }

            return toR;
        }

        /// <summary>
        /// Retrieves a list of convex corners within a specified room.
        /// </summary>
        /// <param name="roomID">The identifier of the room.</param>
        /// <returns>A list of Vector2Int representing convex corners positions.</returns>
        internal List<Vector2Int> GetConvexCorners(int roomID)
        {
            var pairs = rooms[roomID];
            var corners = new List<Vector2Int>();
            foreach (var (pos, tile) in pairs)
            {
                var value = NeigthborValue(pos.x, pos.y);

                if (NumbersSet.IsConvexCorner(value))
                {
                    corners.Add(pos);
                }
            }
            return corners;
        }

        /// <summary>
        /// Retrieves a list of concave corners within a specified room.
        /// </summary>
        /// <param name="roomID">The identifier of the room.</param>
        /// <returns>A list of Vector2Int representing convex corners positions.</returns>
        internal List<Vector2Int> GetConcaveCorners(int roomID)
        {
            var pairs = rooms[roomID];
            var corners = new List<Vector2Int>();

            foreach (var (pos, tile) in pairs)
            {
                var value = NeigthborValue(pos.x, pos.y);

                if (NumbersSet.IsConcaveCorner(value))
                {
                    corners.Add(pos);
                }
            }
            return corners;
        }

        /// <summary>
        /// Get a list of points that form the wall and the direction of the wall.
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        [Obsolete()]
        private List<(Vector2Int[], Vector2Int)> GetVerticalWalls(int roomID)
        {
            var room = rooms[roomID];
            var walls = new List<(Vector2Int[], Vector2Int)>();

            var convexCorners = GetConvexCorners(roomID);
            var allCorners = GetConcaveCorners(roomID);
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    if (current.x - candidate.x != 0) // Comprobación para pared vertical
                        continue;

                    var dist = Mathf.Abs(current.y - candidate.y);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.Item1.First() == other) && (w.Item1.Last() == current))) // Unnecessary?
                    continue;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Max(current.y, other?.y ?? 0);
                var start = Mathf.Min(current.y, other?.y ?? 0);

                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(current.x, start + i));
                }

                bool toRight = true;
                for (int i = 0; i < wallTiles.Count; i++)
                {
                    var n = wallTiles[i] + Vector2Int.right;
                    if (room.ContainsKey(n))
                    {
                        toRight = false;
                        break;
                    }
                }

                var dir = (toRight) ? Vector2Int.right : Vector2Int.left; // Cambiado para muro vertical
                walls.Add((wallTiles.ToArray(), dir));
            }
            return walls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        [Obsolete]
        private List<(Vector2Int[], Vector2Int)> GetHorizontalWalls(int roomID)
        {
            var room = rooms[roomID];
            var walls = new List<(Vector2Int[], Vector2Int)>();

            var convexCorners = GetConvexCorners(roomID);
            var allCorners = GetConcaveCorners(roomID);
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    if (current.y - candidate.y != 0)
                        continue;

                    var dist = Mathf.Abs(current.x - candidate.x);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.Item1.First() == other) && (w.Item1.Last() == current))) // UNESESARY?
                    continue;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Max(current.x, other?.x ?? 00);
                var start = Mathf.Min(current.x, other?.x ?? 00);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(start + i, current.y));
                }

                bool toUp = true;
                for (int i = 0; i < wallTiles.Count; i++)
                {
                    var n = wallTiles[i] + Vector2Int.up;
                    if (room.ContainsKey(n))
                    {
                        toUp = false;
                        break;
                    }
                }

                var dir = (toUp) ? Vector2Int.up : Vector2Int.down;
                walls.Add((wallTiles.ToArray(), dir));
            }
            return walls;
        }

        /// <summary>
        /// Set the room id to the tiles in the rectangle,
        /// and recalculate the neigthbors and walls.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        public void SetRoomTiles(Vector2Int min, Vector2Int max, int value)
        {
            var pos = new List<Vector2Int>();
            for (int i = min.x; i < max.x; i++)
            {
                for (int j = min.y; j < max.y; j++)
                {
                    pos.Add(new Vector2Int(i, j));
                }
            }
            SetRoomTiles(pos, value);
        }

        /// <summary>
        /// Return the number of neigthbors that the tile has in the 8 directions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int NeigthborValue(int x, int y)
        {
            var bitArray = new BitArray(8);
            for (int i = 0; i < Directions.directions_8.Count; i++)
            {
                var dir = Directions.directions_8[i];
                var nx = x + dir.x;
                var ny = y + dir.y;

                foreach (var r in rooms)
                {
                    var pos = new Vector2Int(nx, ny);
                    if (r.Value.ContainsKey(pos))
                    {
                        bitArray[i] = true;
                    }
                }
            }

            return GeneralUtils.BitArrayToInt(bitArray);
        }

        /// <summary>
        /// Return the number of walls that the tile has in the 4 directions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int WallsValue(int x, int y)
        {
            var toR = 0;
            for (int i = 0; i < Directions.directions_4.Count; i++)
            {
                var dir = Directions.directions_4[i];
                var nx = x + dir.x;
                var ny = y + dir.y;

                foreach (var r in rooms)
                {
                    var pos = new Vector2Int(nx, ny);
                    if (r.Value.ContainsKey(pos))
                    {
                        toR++;
                    }
                }
            }
            return toR;
        }

        /// <summary>
        /// Converts the rooms of the map into a tile matrix and returns the matrix
        /// along with its dimensions.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><b>Matrix of Tile:</b> A matrix of Tile objects representing 
        /// the map, where each position corresponds to a tile on the map.</description></item>
        /// <item><description><b>Width:</b> The width of the matrix, which is the number
        /// of columns in the tile matrix.</description></item>
        /// <item><description><b>Height:</b> The height of the matrix, which is the number 
        /// of rows in the tile matrix.</description></item>
        /// </list>
        /// </returns>
        public ((Vector2Int[,], int[,], Tile[,]), int, int) ToTileMatrix()
        {
            var rect = Bounds;
            var tiles = new Tile[rect.width + 1, rect.height + 1];
            var cords = new Vector2Int[rect.width + 1, rect.height + 1];
            var roomID = new int[rect.width + 1, rect.height + 1];

            foreach (var r in rooms)
            {
                foreach (var t in r.Value)
                {
                    var pivot = t.Key - rect.min;
                    tiles[pivot.x, pivot.y] = t.Value;
                    cords[pivot.x, pivot.y] = t.Key;
                    roomID[pivot.x, pivot.y] = r.Key;
                }
            }
            return ((cords, roomID, tiles), rect.width + 1, rect.height + 1);
        }

        public static Map MatrixToMap(int[,] matrix, int w, int h)
        {
            var map = new Map();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (matrix[j, i] == 0)
                        continue;

                    var tile = new Tile(map);
                    tile.roomID = matrix[j, i];
                    if (!map.rooms.ContainsKey(tile.roomID))
                    {
                        map.rooms.Add(tile.roomID, new Dictionary<Vector2Int, Tile>());
                    }
                    map.rooms[tile.roomID].Add(new Vector2Int(j, i), tile);
                }
            }
            return map;
        }

        public (List<Vector2Int>, List<Tile>) GetNeigTiles(Vector2Int pos, List<Vector2Int> dirs)
        {
            var posR = new List<Vector2Int>();
            var tilesR = new List<Tile>();

            foreach (var d in dirs)
            {
                var dd = pos + d;

                foreach (var r in rooms)
                {
                    posR.Add(dd);
                    tilesR.Add(r.Value.ContainsKey(dd) ? r.Value[dd] : null);
                }
            }

            return (posR, tilesR);
        }

        /// <summary>
        /// Print the map in the console.
        /// </summary>
        public void Print()
        {
            var ((cords, rooms, tiles), w, h) = ToTileMatrix();

            var msg = "\n";
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    msg += rooms[i, j] + ", ";
                }
                msg += "\n";
            }
            Debug.Log(msg);
        }

        public (int, Dictionary<Vector2Int, Tile>) GetRoom(Vector2Int vector2Int)
        {
            foreach (var r in rooms)
            {
                if (r.Value.ContainsKey(vector2Int))
                {
                    return (r.Key, r.Value);
                }
            }
            return (-1, null);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            var other = obj as Map;

            // check the type of obj
            if (other == null) return false;

            // check the amount of rooms is the same
            if (this.rooms.Count != other.rooms.Count)
                return false;

            foreach (var (key, value) in this.rooms)
            {
                // check if the other map has the same room ids
                if (other.rooms.TryGetValue(key, out var oRoom))
                {
                    // check if the room has the same amount of tiles
                    if (value.Count != oRoom.Count)
                        return false;

                    // check if the tiles are the same
                    foreach (var (pos, tile) in value)
                    {
                        if (!oRoom.TryGetValue(pos, out var t))
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            // Start with a hash code of a prime number
            int hash = 17;

            // Combine the hash codes of all the rooms and their tiles
            foreach (var room in rooms)
            {
                hash = hash * 31 + room.Key.GetHashCode();

                foreach (var tile in room.Value)
                {
                    hash = hash * 31 + tile.Key.GetHashCode();
                }
            }

            return hash;
        }

        #endregion
    }
}