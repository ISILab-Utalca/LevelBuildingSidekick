using Optimization.Data;
using Optimization.Evaluators;
using Optimization.Restrictions;
using Optimization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
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

            foreach (var r in map.rooms)
            {
                foreach (var (pos, tile) in r.Value)
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
                var c = map.GetConcaveCorners(id); // FIX?: no esta encontrando esquinas en el mapa de points
                c.AddRange(map.GetConvexCorners(id)); // FIX?: no esta encontrando esquinas en el mapa de points

                n += c.Count;
            }

            return 1 - (n / max);
            //return 1 - ((n - min) / (max - min)); // FIX: si n es menor que min, el resutlado mas que 1
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

                var mArea = GetRoomArea(room);
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

        public Vector2Int GetRoomArea(Dictionary<Vector2Int, Tile> room)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var (pos, tile) in room)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }
            return (new Vector2Int(maxX - minX + 1, maxY - minY + 1));
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
                if (room.Count() == 0)
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
                toCheck.Enqueue(room.First().Key);

                while (toCheck.Count > 0)
                {
                    var current = toCheck.Dequeue();
                    _checked.Add(current);

                    // por cada vecino del tile actual
                    foreach (var dir in Directions.directions_4)
                    {
                        var n = current + dir;

                        if (!room.ContainsKey(n))
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

                if(room.Count() != _checked.Count())
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
                foreach (var (pos,tile) in room1)
                {
                    foreach (var dir in Directions.directions_4)
                    {
                        var n = pos + dir;
                        if (room2.ContainsKey(n))
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