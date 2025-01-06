using Optimization.Data;
using Optimization.Evaluators;
using Optimization.Neigbors;
using Optimization.Restrictions;
using Optimization.Terminators;
using Optimization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Optimization.ACO
{

    public class _Data // TODO: implementar despues de probar que el esxperimento funciona
    {
        public struct generacion
        {
            public float totalTime;
            public float explorationTime;
            public float evaluatorTime;

            public float genBest;
            public float average;

            // podria sacar la evaluacion de cada uno de los optimizadores

            public int niegExplored;
            public int successfulPathCount;
            public int deadEndCount;
        }

        public (List<Map>, float) best;
        public int totalNeigsExplored;
        public List<generacion> generations = new();

        // tiempo total de ejecucion
        public float totalTime;
    }

    public class ACO
    {
        public Dictionary<Map, Dictionary<Map, float>> mapNeigs = new(); // from -> L(to, pheromone)

        public bool enforceGraph = true;

        public _Data data;

        public (List<Map>, _Data) Execute(Graph graph, int antCount, float pherIntnesity, float disipacion, IEvaluator evaluator, ITerminator terminator, IRestriction restriction)
        {
            var data = new _Data();// <- For TESTING

            float bestEval = float.MinValue;
            List<Map> best = new();
            float bestNCEval = float.MinValue;
            List<Map> bestNoCompleted = new();

            // obtengo los nodos ordenados por la cantidad de vecinos
            var predeterminePath = GetNodeSortedByNeigs(graph);

            var swTotal = new System.Diagnostics.Stopwatch(); // <- For TESTING
            swTotal.Start();// <- For TESTING

            var prevExplored = 0;

            // cata iteracion del while es una nueva generacion de hormigas
            while (!terminator.Execute())
            {
                prevExplored = mapNeigs.Count;
                float bestGenEval = float.MinValue;
                List<Map> bestGen = new();
                float averageCumulated = 0;

                var gen = new _Data.generacion();
                var swGen = new System.Diagnostics.Stopwatch(); // <- For TESTING
                swGen.Start();// <- For TESTING

                var paths = new List<List<Map>>(); // last node added, currentmap

                var swExplore = new System.Diagnostics.Stopwatch(); // <- For TESTING
                swExplore.Start();// <- For TESTING
                                  // cada hormiga explora un camino hasta llegar a completar un mapa con todas las habitaciones
                for (int i = 0; i < antCount; i++)
                {
                    var path = AntPath(graph, predeterminePath, restriction);
                    paths.Add(path);
                }
                swExplore.Stop();// <- For TESTING
                gen.explorationTime = swExplore.ElapsedMilliseconds;// <- For TESTING

                var swEval = new System.Diagnostics.Stopwatch(); // <- For TESTING
                swEval.Start();// <- For TESTING
                               // por cada camino regreso las hormigas y aplico la pheromona
                foreach (var path in paths)
                {
                    // si la hormiga no termino el camino
                    if (path.Count < graph.nodes.Count)
                    {
                        var lastNC = path.Last();
                        var evNC = evaluator.Execute(lastNC);

                        if (bestNCEval < evNC)
                        {
                            bestNCEval = evNC;
                            bestNoCompleted = path;
                        }
                        gen.deadEndCount++; // <- For TESTING
                        continue; // no suma feromonas
                    }
                    else
                    {
                        gen.successfulPathCount++; // <- For TESTING
                    }

                    var last = path.Last();

                    // evaluo el camino
                    var pathEval = evaluator.Execute(last);
                    averageCumulated += pathEval;

                    if (bestGenEval < pathEval)
                    {
                        bestGenEval = pathEval;
                        bestGen = path;
                    }

                    // actualizo feromonas
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        var cur = path[i];
                        var next = path[i + 1];

                        // VER Si ESTO es la mejor forma de aumentar el valor de la feromona
                        mapNeigs[cur][next] += (pathEval * pherIntnesity);
                    }
                }
                swEval.Stop();
                gen.evaluatorTime = swEval.ElapsedMilliseconds; // <- For TESTING

                // disminuyo la feromona
                foreach (var (from, edge) in mapNeigs)
                {
                    var ms = edge.Select(e => e.Key).ToList();
                    foreach (var m in ms)
                    {
                        edge[m] *= disipacion;
                    }
                }

                swGen.Stop();
                gen.totalTime = swGen.ElapsedMilliseconds; // <- For TESTING
                if (bestEval < bestGenEval)
                {
                    bestEval = bestGenEval;
                    best = bestGen;
                }
                gen.genBest = bestGenEval; // <- For TESTING
                gen.average = (paths.Count != 0) ? averageCumulated / (paths.Count) : -1; // <- For TESTING
                gen.niegExplored = mapNeigs.Count - prevExplored; // <- For TESTING
                data.generations.Add(gen); // <- For TESTING

            }
            swTotal.Stop(); // <- For TESTING
            data.totalTime = swTotal.ElapsedMilliseconds; // <- For TESTING
            data.totalNeigsExplored = mapNeigs.Count; // <- For TESTING

            if (best.Count != 0)
            {
                data.best = (best, bestEval); // <- For TESTING
                return (best, data);
            }
            else
            {
                data.best = (bestNoCompleted, bestNCEval); // <- For TESTING
                return (bestNoCompleted, data);
            }

        }

        /// <summary>
        /// XXX
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<Map> AntPath(Graph graph, List<(Graph.Node, Graph.Node)> path, IRestriction restriction)
        {
            Map current = GenerateFirst(path[0].Item2);
            var pathList = new List<Map>();
            pathList.Add(current);

            for (int i = 1; i < path.Count; i++)
            {
                var (nPrev, nCurrent) = path[i];

                if (!mapNeigs.ContainsKey(current))
                {
                    mapNeigs.Add(current, new Dictionary<Map, float>());
                }

                //var neigs = mapNeigs[current];
                mapNeigs.TryGetValue(current, out var neigs);

                // si ya explore alguno continua algun de esos 
                var validAmount = ValidsPathAmount(neigs);
                var rValue = UnityEngine.Random.Range(0f, validAmount + 1f);
                if (rValue < validAmount)
                {
                    // si existe elijo por pheromona
                    var select = neigs.ToList().RandomRullete(n => n.Value);
                    pathList.Add(select.Key);
                    current = select.Key;
                }
                else // o decido si seguir explorando
                {
                    // creo un nuevo diccionario
                    var (neig, value) = AntExplore(current, graph, nPrev, nCurrent, restriction);

                    if (value > 0)
                    {
                        pathList.Add(neig);
                        current = neig;
                    }
                    else
                    {
                        return pathList;
                    }
                }
            }

            return pathList;
        }

        public int ValidsPathAmount(Dictionary<Map, float> neigs)
        {
            var sum = 0;
            for (int j = 0; j < neigs.Count; j++)
            {
                sum += (neigs.ElementAt(j).Value > 0) ? 1 : 0;
            }
            return sum;
        }

        private List<Map> GetRoots(Map map)
        {
            var toR = new List<Map>();
            foreach (var (from, to) in mapNeigs)
            {
                if (to.ContainsKey(map))
                {
                    toR.Add(from);
                }
            }
            return toR;
        }

        private Map GenerateFirst(Graph.Node node)
        {
            var init = new Map();
            var current = node;

            // selecciono un tamaño para la nueva habitacion
            var w = UnityEngine.Random.Range((int)node.minArea.x, (int)node.maxArea.x + 1);
            var h = UnityEngine.Random.Range((int)node.minArea.y, (int)node.maxArea.y + 1);

            init.SetRoomTiles(new Vector2Int(0, 0), new Vector2Int(w, h), node.id);

            return init;
        }

        public (Map, float) AntExplore(Map current, Graph graph, Graph.Node nodePrev, Graph.Node nodeCurrent, IRestriction restriction)
        {
            // selecciono un tamaño para la nueva habitacion
            var w = UnityEngine.Random.Range((int)nodeCurrent.minArea.x, (int)nodeCurrent.maxArea.x + 1);
            var h = UnityEngine.Random.Range((int)nodeCurrent.minArea.y, (int)nodeCurrent.maxArea.y + 1);

            // obtengo los posibles pivotes para ese tamaño
            var pivots = GetPivotsNeigs(nodePrev.pos + nodeCurrent.pos, new Vector2Int(w, h), nodePrev.id, current);

            // selecciono uno aleatorio
            var pivot = pivots.GetRandom();
            var other = current.Clone() as Map;
            other.SetRoomTiles(pivot, pivot + nodeCurrent.maxArea, nodeCurrent.id);

            // no son añadidos si no cumplen con la restriccion
            if (restriction.Execute(new Tuple<Map, Graph>(other, graph)))
            {
                if (!mapNeigs[current].ContainsKey(other))
                {
                    mapNeigs[current].Add(other, 1);
                }
                return (other, 1);

            }
            else
            {
                if (!mapNeigs[current].ContainsKey(other))
                {
                    mapNeigs[current].Add(other, 0);
                }
                return (other, 0);
            }
        }

        private List<Map> GenerateNeigs(Map prev, Graph graph, Graph.Node nodePrev, Graph.Node nodeCurrent)
        {
            var toR = new List<Map>();

            // obtengo los posibles pivotes
            var pivots = GetPivotsNeigs(nodePrev.pos + nodeCurrent.pos, nodeCurrent.maxArea, nodePrev.id, prev);

            // por cada pivote
            for (int i = 0; i < pivots.Count; i++)
            {
                var other = prev.Clone() as Map;
                other.SetRoomTiles(pivots[i], pivots[i] + nodeCurrent.maxArea, nodeCurrent.id);
                toR.Add(other);
            }

            return toR;
        }

        /// <summary>
        /// regresa una lista de nodos ordenados desde el nodo con mas vecinos,
        /// para luego obtener los vecinos de este y regresarlos ordenados por
        /// la cantidad de vecinos, asi susesibamente.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns> a tuple [from -> to]</returns>
        private List<(Graph.Node, Graph.Node)> GetNodeSortedByNeigs(Graph graph)
        {
            var added = new List<Graph.Node>();
            var toR = new List<(Graph.Node, Graph.Node)>();
            var toAdd = new Queue<(Graph.Node, Graph.Node)>();

            var first = graph.nodes.OrderByDescending(n => graph.GetNeighbors(n).Count).First();
            toAdd.Enqueue((first, first));

            while (toAdd.Count > 0)
            {
                var (prev, current) = toAdd.Dequeue();

                // si ya esta en la lista lo salto
                if (added.Contains(current))
                    continue;

                toR.Add((prev, current));
                added.Add(current);

                var neigs = graph.GetNeighbors(current);
                neigs = neigs.OrderByDescending((n) =>
                {
                    var amount = added.Intersect(graph.GetNeighbors(n)).Count();
                    return amount;
                }).ToList();
                foreach (var n in neigs)
                {
                    if (!added.Contains(n))
                        toAdd.Enqueue((current, n));
                }
            }
            return toR;
        }

        /// <summary>
        /// con la direccion entre la habitacion que quiero añadir y la habitacion que
        /// ya esta en el mapa, obtengo los posibles pivotes para añadir la nueva habitacion
        /// </summary>
        /// <param name="nodeVector"></param>
        /// <param name="sizeArea"></param>
        /// <param name="roomID"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        private List<Vector2Int> GetPivotsNeigs(Vector2 nodeVector, Vector2Int sizeArea, int roomID, Map map)
        {
            var toR = new List<Vector2Int>();

            // obtengo la direcciones a la que se encuentra la nueva habitacion
            var dirs = !enforceGraph ?
                Directions.AllDirs() :
                Directions.AngulatedDirs(nodeVector);


            // obtengo la habitacion anterior
            var room = map.rooms[roomID];

            foreach (var dir in dirs)
            {
                if (dir == Directions.Dirs_4.None)
                    continue;

                // obtengo las posiciones vacias
                var emptyPos = GetAdjacent(dir, room);

                // obtengo el desplazamiento principal
                var starDisp = (StartDisp(dir) * sizeArea);// - StartDisp(dir);
                                                           //var v1 = Vector2Int.zero;

                // obtengo el desplazamiento secundario
                var endDisp = (EndDisp(dir) * sizeArea);// - EndDisp(dir);

                foreach (var pos in emptyPos)
                {
                    // obtengo la cantidad de desplazamientos secundarios
                    var dispacedPoints = GeneralUtils.GetPointsBetween(pos + starDisp, pos + endDisp);

                    foreach (var point in dispacedPoints)
                    {
                        // obtengo el pivote
                        //var pivot = pos + v2;//v1 + v2;

                        // si no esta en la lista lo añado
                        if (!toR.Contains(point))
                            toR.Add(point);

                        // si no esta en la lista lo añado
                        //if (!toR.Contains(pivot))
                        //    toR.Add(pivot);
                    }

                }
            }

            return toR;  // FIX: los numeros podrian estar mas distantes de lo que deberian
        }

        private List<Vector2Int> GetAdjacent(Directions.Dirs_4 dir, Dictionary<Vector2Int, Tile> room)
        {
            var toR = new List<Vector2Int>();
            foreach (var (pos, tile) in room)
            {
                var adjacentTile = pos + Directions.directions_4[(int)dir];
                Tile t = null;
                if (!room.TryGetValue(adjacentTile, out t))
                {
                    toR.Add(adjacentTile);
                }
            }
            return toR;
        }

        private Vector2Int EndDisp(Directions.Dirs_4 dir)
        {
            switch (dir)
            {
                case Directions.Dirs_4.Up:
                    return new Vector2Int(0, 0);
                case Directions.Dirs_4.Down:
                    return new Vector2Int(0, -1);
                case Directions.Dirs_4.Left:
                    return new Vector2Int(-1, 0);
                case Directions.Dirs_4.Right:
                    return new Vector2Int(0, 0);
                default:
                    return Vector2Int.zero;
            }
        }

        private Vector2Int StartDisp(Directions.Dirs_4 dir)
        {
            switch (dir)
            {
                case Directions.Dirs_4.Up:
                    return new Vector2Int(-1, 0);
                case Directions.Dirs_4.Down:
                    return new Vector2Int(-1, -1);
                case Directions.Dirs_4.Left:
                    return new Vector2Int(-1, -1);
                case Directions.Dirs_4.Right:
                    return new Vector2Int(0, -1);
                default:
                    return Vector2Int.zero;
            }
        }
    }
}