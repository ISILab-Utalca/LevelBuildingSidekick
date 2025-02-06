using Optimization.Data;
using Optimization.Evaluators;
using Optimization.Restrictions;
using Optimization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Optimization.Evaluators
{
    public interface IEvaluator
    {
        public float Execute(object obj);
    }

    public class WeightedAgregateEvaluator : IEvaluator
    {
        public (IEvaluator, float)[] evs;

        public float Execute(object obj)
        {
            if (evs == null || evs.Length == 0)
            {
                Debug.LogWarning("No Tiene Sub-Evalaudores.");
                return -1;
            }

            var total = 0f;
            foreach (var e in evs)
            {
                var (evaluator, weight) = e;
                var value = evaluator.Execute(obj);
                total += value * weight;
            }
            return total;
        }
    }
}

namespace Problem.Evaluators
{
    public class VoidEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var map = (Map)obj;

            float boundArea = map.Width * map.Height;
            float voidArea = boundArea - map.Area;

            return 1f - (voidArea / boundArea);
        }
    }

    public class ExteriorWallEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var map = (Map)obj;

            var n = 0;
            float min = 2f * (map.Width * map.Height); // circumference
            float max = 2f * min;

            foreach (var (key, r) in map.rooms)
            {
                foreach (var (pos, tile) in r.tiles)
                {
                    n += map.WallsValue(pos.x, pos.y); // FIX: no esta sumando nada a n
                }
            }

            return 1f - (n / max);
            //return 1f - ((n - min) / (max - min)); // FIX: si n es menor que min, el resutlado mas que 1
        }
    }

    public class CornerEvaluator : IEvaluator
    {
        public float Execute(object obj) // FIX
        {
            var map = (Map)obj;

            var n = 0;
            float min = 4 * map.rooms.Count;
            float max = map.Height * map.Width;

            foreach (var (id, room) in map.rooms)
            {
                var c = room.GetConcaveCorners(); // FIX?: no esta encontrando esquinas en el mapa de points
                c.AddRange(room.GetConvexCorners()); // FIX?: no esta encontrando esquinas en el mapa de points

                n += c.Count;
            }

            return 1 - (n / max);
            //return 1 - ((n - min) / (max - min)); // FIX: si n es menor que min, el resutlado mas que 1
        }
    }


    public class AdjacenciesEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;

            if (tuple == null) // FIX?: remover este tipo de validaciones.
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return -1;
            }

            var (map, graph) = tuple;

            float distValue = 0f;
            foreach (var edge in graph.edges) // for each conection
            {
                map.rooms.TryGetValue(edge.n1.id, out var room1);
                map.rooms.TryGetValue(edge.n2.id, out var room2);

                if (room1 == null || room2 == null)
                    continue;

                var dist = float.MaxValue;
                foreach (var (pos, tile) in room1.tiles)
                {
                    foreach (var (pos2, tile2) in room2.tiles)
                    {
                        var d = Vector2.Distance(pos, pos2);
                        if (d < dist)
                        {
                            dist = d;
                        }
                    }
                }

                distValue += 1 / dist;
            }

            return distValue / graph.edges.Count;
        }
    }

    public class AreasEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;

            if (tuple == null)
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return -1;
            }

            var (map, graph) = tuple;

            // if no rooms in the map
            if (map.rooms.Count <= 0)
                return 0;

            //
            var value = 0f;
            foreach (var (key,room) in map.rooms)
            {
                var bound = room.Bounds;
                var node = graph.nodes.Find(n => n.id == key);
                var minLimit = node.minArea;
                var maxLimit = node.maxArea;

                value += EvaluateArea(bound, minLimit, maxLimit);
            }


            return value / (map.rooms.Count * 1f);

        }


        public float EvaluateArea(RectInt area, Vector2Int minArea, Vector2Int maxArea)
        {
            var vw = 1f;
            if (area.width > maxArea.x || area.width < minArea.x)
            {
                vw = area.width / ((float)(minArea.x + maxArea.x) / 2f);
                if (vw > 1)
                    vw = 1 / vw;
            }

            var vh = 1f;
            if (area.height > maxArea.y || area.height < minArea.y)
            {
                vh = area.height / ((float)(minArea.y + maxArea.y) / 2f);
                if (vh > 1)
                    vh = 1 / vh;
            }

            return (vw + vh) / 2f;
        }
    }

    public class EmptySpaceEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;

            if (tuple == null) // FIX?: remover este tipo de validaciones.
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return -1;
            }

            var (map, graph) = tuple;

            var avg = 0f;
            foreach (var (key, room) in map.rooms)
            {
                var rect = room.Bounds;

                if (rect.width <= 0 || rect.height <= 0) // FIX?: remover este tipo de validaciones.
                    continue;

                avg += room.tiles.Count / (float)(rect.width * rect.height);
            }
            return avg;
        }
    }

    public class RoomCutEvaluator : IEvaluator
    {
        public float Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;

            if (tuple == null) // FIX?: remover este tipo de validaciones.
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return -1;
            }

            var (map, graph) = tuple;

            if (map.rooms.Count <= 0)
                return 0;

            var value = 0f;
            foreach (var (key, room) in map.rooms)
            {
                var tiles = room.tiles;
                var check = new List<Vector2Int>();
                var uncheck = new Queue<Vector2Int>();

                if (tiles.Count <= 0)
                    continue;

                uncheck.Enqueue(tiles.First().Key);

                do
                {
                    var current = uncheck.First();
                    var (neiPoss,neiTiles) = map.GetTileNeighbors(current, Directions.directions_4);
                    foreach (var nei in neiPoss)
                    {
                        if (!check.Contains(nei) && !uncheck.Contains(nei))
                        {
                            uncheck.Enqueue(nei);
                        }
                    }
                    uncheck.Dequeue();
                    check.Add(current);
                }
                while (uncheck.Count > 0);

                value = (tiles.Count > check.Count) ? 0 : 1;
            }

            return value / (float)map.rooms.Count;
        }
    }
}

namespace Optimization.Restrictions
{
    /// <summary>
    /// Regresan true si se cumple la restriccion.
    /// </summary>
    public interface IRestriction
    {
        public bool Execute(object obj);
    }

    public class AgregateRestriction : IRestriction
    {
        public IRestriction[] res;

        public bool Execute(object obj)
        {
            if (res == null || res.Length == 0) 
            {
                Debug.LogWarning("No Tiene Sub-Restricciones.");
                return true; 
            }

            foreach (var r in res)
            {
                if (!r.Execute(obj))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

namespace Problem.Restrictions
{
    public class MinMaxAreaRestriction : IRestriction
    {
        public bool Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;
            if (tuple == null)
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return false;
            }

            var (map, graph) = tuple;

            foreach (var (id, room) in map.rooms)
            {
                var node = graph.nodes.Where(n => n.id == id).ToList()[0];

                var mArea = room.GetRoomArea();
                if (node.minArea.x <= mArea.x && mArea.x <= node.maxArea.x &&
                    node.minArea.y <= mArea.y && mArea.x <= node.maxArea.x)
                {
                    // do nothing
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class AmountRoomRestriction : IRestriction
    {
        public bool Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;

            if (tuple == null)
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return false;
            }

            var (map, graph) = tuple;

            foreach (var (id, room) in map.rooms)
            {
                if (room.tiles.Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class SplitingRoomRestriction : IRestriction
    {
        public bool Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;
            
            if (tuple == null)
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return false;
            }

            var (map, graph) = tuple;

            // por cada room en el mapa
            foreach (var (id, room) in map.rooms)
            {
                var toCheck = new Queue<Vector2Int>();
                var _checked = new List<Vector2Int>();

                // agrego el primer tile de la room a la lista de tiles por revisar
                toCheck.Enqueue(room.tiles.First().Key);

                while (toCheck.Count > 0)
                {
                    var current = toCheck.Dequeue();
                    _checked.Add(current);

                    // por cada vecino del tile actual
                    foreach (var dir in Directions.directions_4)
                    {
                        var n = current + dir;

                        if (!room.tiles.ContainsKey(n))
                        {
                            continue;
                        }

                        // si el vecino no esta en la lista de tiles por revisar
                        // y no esta en la lista de tiles ya revisados
                        if (!toCheck.Contains(n) && !_checked.Contains(n))
                        {
                            toCheck.Enqueue(n);
                        }
                    }
                }

                if(room.tiles.Count() != _checked.Count())
                {
                    return false;
                }
            }
            return true; 
        }

    }

    public class ConectivityGraphRestriction : IRestriction
    {
        public bool Execute(object obj)
        {
            var tuple = obj as Tuple<Map, Graph>;
            
            if (tuple == null)
            {
                Debug.LogWarning("No es un Tuple<Map, Graph>.");
                return false;
            }

            var (map, graph) = tuple;

            foreach (var edge in graph.edges)
            {
                //var room1 = map.rooms[edge.n1.id];
                map.rooms.TryGetValue(edge.n1.id, out var room1);
                //var room2 = map.rooms[edge.n2.id];
                map.rooms.TryGetValue(edge.n2.id, out var room2);

                if (room1 == null || room2 == null)
                    continue;

                var conected = false;
                foreach (var (pos,tile) in room1.tiles)
                {
                    foreach (var dir in Directions.directions_4)
                    {
                        var n = pos + dir;
                        if (room2.tiles.ContainsKey(n))
                        {
                            conected = true;
                            break;
                        }
                    }
                    if (conected) break;
                }

                if (!conected)
                {
                    return false;
                }
            }

            return true; 
          
        }



    }
}