using ISILab.AI.Optimization;
using Optimization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Optimization.Data
{
    public class Tile : ICloneable
    {
        public Room owner;

        #region Constructors
        public Tile(Room owner)
        {
            this.owner = owner;
        }

        public object Clone() 
        {
            return new Tile(owner);// TODO: Impelentar el sistema de diccionario que permite clonar referencias.
        }
        #endregion
    }

    public class Room : ICloneable
    {
        public Map owner;
        public Dictionary<Vector2Int, Tile> tiles = new();

        public RectInt Bounds
        {
            get
            {
                if (tiles.Count <= 0)
                    return new RectInt(0, 0, 0, 0);

                var min = new Vector2Int(int.MaxValue, int.MaxValue);
                var max = new Vector2Int(int.MinValue, int.MinValue);

                foreach (var (key, tile) in tiles)
                {
                    if (key.x < min.x)
                        min.x = key.x;
                    if (key.x > max.x)
                        max.x = key.x;

                    if (key.y < min.y)
                        min.y = key.y;
                    if (key.y > max.y)
                        max.y = key.y;
                }

                return new RectInt(min.x, min.y, max.x - min.x, max.y - min.y);
            }
        }

        public Vector2Int GetRoomArea()
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var (pos, tile) in tiles)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }
            return (new Vector2Int(maxX - minX + 1, maxY - minY + 1));
        }

        /// <summary>
        /// Retrieves a list of convex corners within a specified room.
        /// </summary>
        /// <param name="roomID">The identifier of the room.</param>
        /// <returns>A list of Vector2Int representing convex corners positions.</returns>
        internal List<Vector2Int> GetConvexCorners()
        {
            var corners = new List<Vector2Int>();
            foreach (var (pos, tile) in tiles)
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
        internal List<Vector2Int> GetConcaveCorners()
        {
            var corners = new List<Vector2Int>();
            foreach (var (pos, tile) in tiles)
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
        /// 
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public List<(Vector2Int[], Vector2Int)> GetWalls()
        {
            var toR = new List<(Vector2Int[], Vector2Int)>();
            var candidates = new Dictionary<(Vector2Int, Directions.Dirs_4), Vector2Int>();

            var concave = GetConcaveCorners();
            var convex = GetConvexCorners();
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
                    if (tiles.ContainsKey(other))
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

                var pos = new Vector2Int(nx, ny);
                if (tiles.ContainsKey(pos))
                {
                    bitArray[i] = true;
                }
            }

            return GeneralUtils.BitArrayToInt(bitArray);
        }

        public Room(Map owner)
        {
            this.owner = owner;
        }

        public object Clone()
        {
            var room = new Room(this.owner); // TODO: Impelentar el sistema de diccionario que permite clonar referencias.

            var tiles = new Dictionary<Vector2Int, Tile>();
            foreach (var (key, value) in this.tiles)
            {
                var t = value.Clone() as Tile;
                tiles.Add(key, t);
            }
            room.tiles = tiles;

            return room;
        }
    }

    public class Map : ICloneable, IOptimizable
    {
        #region Properties
        public RectInt Bounds
        {
            get
            {
                var min = new Vector2Int(int.MaxValue, int.MaxValue);
                var max = new Vector2Int(int.MinValue, int.MinValue);

                foreach (var (key, room) in rooms)
                {
                    foreach (var t in room.tiles)
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

                foreach (var (key, room) in rooms)
                {
                    foreach (var t in room.tiles)
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

                foreach (var (key, room) in rooms)
                {
                    foreach (var t in room.tiles)
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
                foreach (var (key, room) in rooms)
                {
                    foreach (var t in room.tiles)
                    {
                        sum.x += t.Key.x;
                        sum.y += t.Key.y;
                        i++;
                    }
                }
                return new Vector2Int(sum.x / i, sum.y / i);
            }
        }

        public int Area
        {
            get
            {
                var area = 0;
                foreach (var (key, room) in rooms)
                {
                    foreach (var t in room.tiles)
                    {
                        area++;
                    }
                }
                return area;
            }
        }

        public double Fitness { get; set; }
        #endregion

        #region Variables
        public Dictionary<string, Room> rooms = new();
        #endregion

        #region Constructors
        public Map()
        {

        }

        public object Clone()
        {
            var map = new Map();

            var rooms = new Dictionary<string, Room>();
            foreach (var (key,room) in this.rooms)
            {
                var r = room.Clone();
                map.rooms.Add(key, r as Room);
            }
            map.rooms = rooms;

            return map;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Set the room id to the tiles in the positions list,
        /// and recalculate the neigthbors and walls.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="id"></param>
        public void SetRoomTiles(List<Vector2Int> positions, string id)
        {
            // set room owner id
            for (int i = 0; i < positions.Count; i++)
            {
                var x = positions[i].x;
                var y = positions[i].y;
                var pos = new Vector2Int(x, y);

                Tile tile = null;
                foreach (var (key, r) in rooms)
                {
                    if (r.tiles.ContainsKey(pos)) // if exist, extract from previous room
                    {
                        tile = r.tiles[pos];
                        r.tiles.Remove(pos);
                        break;
                    }
                }

                var room = rooms.TryGetValue(id, out var _room) ? _room : new Room(this);

                if (tile == null)
                {
                    tile = new Tile(room);
                    rooms.Add(id, room);
                }

                rooms[id].tiles.Add(pos, tile);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="dir"></param>
        public void MoveArea(string id, Vector2Int dir)
        {
            rooms.TryGetValue(id, out var room);

            var olds = room.tiles;
            room.tiles = new Dictionary<Vector2Int, Tile>();

            var newPositions = olds.Select(t => t.Key + dir).ToList();
            room.owner.SetRoomTiles(newPositions, id);
        }

        /// <summary>
        /// Set the room id to the tiles in the rectangle.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="id"></param>
        public void SetRoomTiles(Vector2Int min, Vector2Int max, string id)
        {
            var pos = new List<Vector2Int>();
            for (int i = min.x; i < max.x; i++)
            {
                for (int j = min.y; j < max.y; j++)
                {
                    pos.Add(new Vector2Int(i, j));
                }
            }
            SetRoomTiles(pos, id);
        }

        public void RemoveTiles()
        {

        }

        /// <summary>
        /// Return the number of neigthbors that the tile has in the 8 directions
        /// considering all rooms.
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

                foreach (var (key,r) in rooms)
                {
                    var pos = new Vector2Int(nx, ny);
                    if (r.tiles.ContainsKey(pos))
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

                foreach (var (key, r) in rooms)
                {
                    var pos = new Vector2Int(nx, ny);
                    if (r.tiles.ContainsKey(pos))
                    {
                        toR++;
                    }
                }
            }
            return toR;
        }

        /// <summary>
        /// Get the neigbours of a tile in the given directions.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="directions"></param>
        /// <returns></returns>
        public (List<Vector2Int>,List<Tile>) GetTileNeighbors(Vector2Int position, List<Vector2Int> directions)
        {
            var toR = (new List<Vector2Int>(),new List<Tile>());

            var neigPositions = directions.Select(d => position + d).ToArray();

            foreach (var (key, room) in rooms)
            {
                foreach (var neigPos in neigPositions)
                {
                    room.tiles.TryGetValue(neigPos, out var tile);

                    if (tile != null)
                    {
                        toR.Item1.Add(neigPos);
                        toR.Item2.Add(tile);
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
        public ((Vector2Int[,], string[,], Tile[,]), int, int) ToTileMatrix()
        {
            var rect = Bounds;
            var tiles = new Tile[rect.width + 1, rect.height + 1];
            var cords = new Vector2Int[rect.width + 1, rect.height + 1];
            var roomID = new string[rect.width + 1, rect.height + 1];

            foreach (var (key,r) in rooms)
            {
                foreach (var t in r.tiles)
                {
                    var pivot = t.Key - rect.min;
                    tiles[pivot.x, pivot.y] = t.Value;
                    cords[pivot.x, pivot.y] = t.Key;
                    roomID[pivot.x, pivot.y] = key;
                }
            }
            return ((cords, roomID, tiles), rect.width + 1, rect.height + 1);
        }

        /// <summary>
        /// Generate a map based on a matrix of strings representing the rooms IDs.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Map MatrixToMap(string[,] matrix, int w, int h)
        {
            var map = new Map();
            var rooms = new Dictionary<string, Room>();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (string.IsNullOrEmpty(matrix[j, i]))
                        continue;

                    rooms.TryGetValue(matrix[j, i], out var room);
                    if(room == null)
                    {
                        rooms.Add(matrix[j, i], new Room(map));
                    }

                    var tile = new Tile(room);
                    room.tiles.Add(new Vector2Int(j, i), tile);
                }
            }
            return map;
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

        public (string, Room) GetRoom(Vector2Int vector2Int)
        {
            foreach (var (key, r) in rooms)
            {
                if (r.tiles.ContainsKey(vector2Int))
                {
                    return (key, r);
                }
            }
            return ("", null);
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
                other.rooms.TryGetValue(key, out var oRoom);

                if (oRoom == null || !oRoom.Equals(value)) // TODO: Implement Room.Equals method
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            // Start with a hash code of a prime number
            int hash = 17;

            // Combine the hash codes of all the rooms and their tiles
            foreach (var (key,room) in rooms)
            {
                hash = hash * 31 + key.GetHashCode();

                foreach (var tile in room.tiles)
                {
                    hash = hash * 31 + tile.Key.GetHashCode();
                }
            }

            return hash;
        }

        public IOptimizable CreateNew()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}