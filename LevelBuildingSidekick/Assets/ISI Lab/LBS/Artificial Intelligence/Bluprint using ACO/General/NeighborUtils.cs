using Optimization.Data;
using Optimization.Neigbors;
using Optimization.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Problem.Neigbors
{
    public class GetNeigborTabu : IGetNeighbors
    {
        /*
        public List<Map> TabuNeigbor((List<int>, List<Vector2Int>) tabu)
        {
            throw new System.NotImplementedException();
        }
        */
        public List<(object, string)> Execute(object obj)
        {
            throw new System.NotImplementedException();
        }
    }

    public class GetNeighborByMoveWalls : IGetNeighbors
    {
        public List<(object, string)> Execute(object obj)
        {
            var map = obj as Map;
            var dirs = Directions.directions_4; // remove this
            var toR = new List<(object, string)>();

            foreach (var (id, room) in map.rooms)
            {
                var walls = map.GetWalls(id);
                foreach (var (point, dir) in walls)
                {
                    List<(object, string)> neig = new();
                    neig = NeighborUtils.GetNeigsByExpandWall(map, id, point.ToList(), dir); // OPTIMIZE: Using Point.toList() may slow down the process, measure time!
                    toR.AddRange(neig);
                    neig = NeighborUtils.GetNeigsByRetractWall(map, id, point.ToList(), dir); // OPTIMIZE: Using Point.toList() may slow down the process, measure time!
                    toR.AddRange(neig);
                }
            }

            for (int i = toR.Count() - 1; i >= 0; i--)
            {
                var (m,s) = toR[i];
                foreach (var (id,rooms) in (m as Map).rooms)
                {
                    if (rooms.Count() == 0)
                    {
                        toR.RemoveAt(i);
                        break;
                    }
                }
            }

            return toR;
        }
    }

    public class GetNeighborByMoveRooms : IGetNeighbors
    {
        public List<(object, string)> Execute(object obj)
        {
            var map = obj as Map;
            var dirs = Directions.directions_4;
            var toR = new List<(object, string)>();

            foreach (var r in map.rooms) // OPTIMIZE: This can be parallelized
            {
                var neig = NeighborUtils.GetNeigsByMoveRoom(map, r.Key, dirs);
                toR.AddRange(neig);
            }
            return toR;
        }
    }

    public static class NeighborUtils
    {
        /// <summary>
        /// Get neigbour expanding walls.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="roomID"></param>
        /// <param name="wall"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static List<(object, string)> GetNeigsByExpandWall(this Map map, int roomID, List<Vector2Int> wall, Vector2Int dir)
        {
            var toR = new List<(object, string)>();

            var clone = map.Clone() as Map;
            var room = clone.rooms[roomID];
            var newWall = wall.Select(p => p + dir).ToList(); // OPTIMIZE: .toList() may slow down the process, measure time!
            clone.SetRoomTiles(newWall, roomID);
            toR.Add((clone, "R:" + roomID + "D:" + dir.ToString() + "M:Expand"));

            return toR;
        }

        /// <summary>
        /// Get neigbour retracting walls.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="roomID"></param>
        /// <param name="wall"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static List<(object, string)> GetNeigsByRetractWall(this Map map, int roomID, List<Vector2Int> wall, Vector2Int dir)
        {
            var toR = new List<(object, string)>();

            var clone = map.Clone() as Map;
            var room = clone.rooms[roomID];

            var frontWall = wall.Select(p => p + dir).ToList(); // OPTIMIZE: .toList() may slow down the process, measure time!

            for (int i = 0; i < frontWall.Count(); i++)
            {
                if(i == 0 || i == frontWall.Count() -1)
                {
                    if (NumbersSet.IsConcaveCorner(clone.NeigthborValue(wall[i].x, wall[i].y)))
                    {
                        continue; // preguntar si es concavo,si lo es me lo salto
                    }
                }

                var (id,dict) = clone.GetRoom(frontWall[i]);
                if (id != -1) 
                {
                    // OPTIMIZE: esto podria pasar sin los chequeos internos de setroom
                    // por que ya seque room le pertence los tiles
                    clone.SetRoomTiles(new List<Vector2Int>() { wall[i] }, id); 
                }
                else 
                {
                    room.Remove(wall[i]);
                }
            }

            toR.Add((clone, "R:" + roomID + "D:" + dir.ToString() + "M:Retract"));

            return toR;
        }

        /// <summary>
        /// Move a room in the map in the given directions.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="roomID"></param>
        /// <param name="dirs"></param>
        /// <returns></returns>
        internal static List<(object, string)> GetNeigsByMoveRoom(this Map map, int roomID, List<Vector2Int> dirs)
        {
            var room = map.rooms[roomID];
            var neigs = new List<(object, string)>();
            List<Vector2Int> originPos = room.Select(p => p.Key).ToList(); // OPTIMIZE: .toList() may slow down the process, measure time!

            for (int i = 0; i < dirs.Count; i++)
            {
                var neigPos = originPos.Select(p => p + dirs[i]).ToList(); // OPTIMIZE: .toList() may slow down the process, measure time!

                var (intersect, toAdd, toRemove) = SegregatePos(neigPos, originPos);

                var neig = map.Clone() as Map;

                var nr = neig.rooms[roomID];
                foreach (var p in toRemove)
                {
                    nr.Remove(p);
                }

                neig.SetRoomTiles(toAdd, roomID);
                neigs.Add((neig, "R:" + roomID + "D:" + dirs[i].ToString()));
            }
            return neigs;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="neigPos"></param>
        /// <param name="originPos"></param>
        /// <returns></returns>
        private static (List<Vector2Int>, List<Vector2Int>, List<Vector2Int>) SegregatePos(List<Vector2Int> neigPos, List<Vector2Int> originPos)
        {
            var intersect = neigPos.Intersect(originPos).ToList();
            var toA = neigPos.Except(intersect).ToList();
            var toR = originPos.Except(intersect).ToList();

            return (intersect, toA, toR);
        }
    }
}

namespace Optimization.Neigbors
{
    public interface IGetNeigbor
    {
        public (object, string) Execute(object obj);
    }

    public interface IGetNeighbors
    {
        public List<(object,string)> Execute(object obj);
    }

    /// <summary>
    /// Return a list of neighbours by given "GetNeighbors".
    /// </summary>
    public class AgregateNeigbors : IGetNeighbors
    {
        public IGetNeighbors[] neigbors;

        public List<(object, string)> Execute(object obj)
        {
            var toR = new List<(object, string)>();
            foreach (var n in neigbors)
            {
                toR.AddRange(n.Execute(obj));
            }
            return toR;
        }
    }
}


