using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.Commons;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    [System.Serializable]
    public class DCExploration : IContextualEvaluator, IRangedEvaluator
    {
        // Weird or inconsistent behaviour? Maybe you just added a new Property and forgot to assign it in the Initialization or Clone Methods, you silly cat!

        public float MaxValue => 1;

        public float MinValue => 0;

        public List<LBSLayer> ContextLayers { get; set; } = new List<LBSLayer>();

        public LBSLayer CombinedInteriorLayer { get; set; } = null;

        public string Tooltip => "DC Exploration Evaluator\n\n" +
            "This evaluator aims to balance the distances between every player and every \"point of interest\" such as chests, weapons and other resources, in order to maximize the explorable space.\n\n" +
            "This evaluator currently only supports Interior Layers as Context.";

        [SerializeField, SerializeReference]
        public LBSCharacteristic playerCharacteristic;

        [SerializeField, SerializeReference]
        public LBSCharacteristic colliderCharacteristic;

        public List<LBSCharacteristic> pointsOfInterest = new List<LBSCharacteristic>();

        public float Evaluate(IOptimizable evaluable)
        {
            var chrom = evaluable as BundleTilemapChromosome;

            if (chrom == null)
            {
                throw new Exception("Wrong Chromosome Type");
            }
            if (chrom.IsEmpty())
            {
                return 0.0f;
            }

            LBSLayer layer = CombinedInteriorLayer;// ContextLayers.FirstOrDefault(l => l.ID.Equals("Interior"));
            
            float fitness = 0;

            var genes = chrom.GetGenes().Cast<BundleData>().ToList();

            List<int> POIs = new List<int>();

            for (int i = 0; i < genes.Count; i++)
            {
                if (chrom.IsInvalid(i))
                    continue;
                if (genes[i] != null)
                {
                    if (genes[i].Characteristics.Contains(playerCharacteristic))
                    {
                        POIs.Add(i);
                        continue;
                    }
                    foreach(var LBSChar in pointsOfInterest)
                    {
                        if(genes[i].Characteristics.Contains(LBSChar))
                        {
                            POIs.Add(i);
                            break;
                        }
                    }
                }
            }

            int size = POIs.Count;

            if (size <= 0)
            {
                Debug.LogWarning("No Points of Interest were found. Try adding a player and some more resource elements. Check the DC Exploration evaluator description for more info.");
                return 0.0f;
            }

            int[,] distances = new int[size, size];
            bool[,] toIgnore = new bool[size, size];

            if (layer != null)
            {
                if(layer.ID.Equals("Interior"))
                    for (int i = 0; i < size; i++)
                    {
                        FloodFill(POIs[i], POIs, i, ref distances, chrom, layer.GetModule<SectorizedTileMapModule>(), layer.GetModule<ConnectedTileMapModule>());
                    }
                else
                    for (int i = 0; i < size; i++)
                    {
                        Manhattan(POIs[i], POIs, i, ref distances, chrom);
                    }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    Manhattan(POIs[i], POIs, i, ref distances, chrom);
                }
            }

            for(int i = 0; i < size; i++)
            {
                break;
                for(int j = 0; j < size; j++)
                {
                    // Llenar toIgnore
                }
            }

            float max = 0;
            foreach(float i in distances)
                if(i > max)
                    max = i;

            List<float> score = new List<float>();
            float sum = 0;
            
            for(int i = 0; i < distances.GetLength(0); i++)
            {
                for(int j = i+1; j < distances.GetLength(1); j++)
                {
                    float newScore = (float)distances[i, j] / max;
                    sum += newScore;
                    score.Add(newScore);
                }
            }

            fitness = sum / (float)score.Count;
            UnityEngine.Assertions.Assert.IsTrue(float.IsNormal(fitness));
            return fitness;
        }

        public void FloodFill(int startPos, List<int> others, int from, ref int[,] distances, BundleTilemapChromosome chrom, SectorizedTileMapModule tileMap, ConnectedTileMapModule connectedTM)
        {
            //maxDistance = 0;
            if (from >= others.Count)
                return;

            List<int> remainingOthers = new List<int>(others);
            remainingOthers.RemoveRange(0, from);
            remainingOthers.Remove(startPos);

            //var distFromStart = new Dictionary<int, int>();
            var remaining = new List<int>();
            var closed = new List<int>();
            foreach (var tile in tileMap.PairTiles.Select(tzp => tzp.Tile))
            {
                int index = chrom.ToIndex(tile.Position - chrom.Rect.position);
                //distFromStart.Add(index, int.MaxValue);
                if (index < 0) continue;
                remaining.Add(index);
            }

            var remainingStep = new Queue<int>();
            remainingStep.Enqueue(startPos);

            int i;
            for (i = 0; remaining.Count > 0; i++)
            {
                if (remainingStep.Count == 0)
                    break;
                List<int> nextStep = new List<int>();
                while(remainingStep.Count > 0)
                {
                    int current = remainingStep.Dequeue();
                    Vector2Int currentPos = chrom.ToMatrixPosition(current) + Vector2Int.RoundToInt(chrom.Rect.position);
                    Zone currentZone = tileMap.GetZone(currentPos);
                    //distFromStart[current] = i;
                    remaining.Remove(current);
                    closed.Add(current);

                    List<Vector2Int> dirs = Directions.Bidimencional.Edges;
                    foreach (Vector2Int dir in dirs)
                    {
                        LBSTile currentTile = tileMap.PairTiles.First(tzp => tzp.Tile.Position == currentPos).Tile;
                        string currentConnection = connectedTM.GetConnections(currentTile)[dirs.FindIndex(d => d.Equals(dir))];
                        if (!(currentConnection.Equals("Door") || currentConnection.Equals("Empty")))
                            continue;

                        Vector2Int newPos = currentPos + dir;

                        int index = chrom.ToIndex(newPos - chrom.Rect.position);

                        //if (tileMap.Contains(pos)) // Esto esta mal. Esto es todo el mapa, no solo la seccion seleccionada
                        if (index < 0 || nextStep.Contains(index) || closed.Contains(index) || chrom.IsInvalid(index))
                            continue;

                        Zone otherZone = tileMap.GetZone(newPos);

                        LBSTile newTile = tileMap.PairTiles.First(tzp => tzp.Tile.Position == newPos).Tile;
                        string connection = connectedTM.GetConnections(newTile)[dirs.FindIndex(d => d.Equals(-dir))];
                        if (!(connection.Equals("Door") || connection.Equals("Empty")))
                            continue;

                        for (int j = from; j < others.Count; j++)
                        {
                            if (index == others[j])
                            {
                                distances[from, j] = distances[j, from] = i + 1;
                                remainingOthers.Remove(index);
                                if (remainingOthers.Count == 0)
                                {
                                    //Debug.Log("i = " + i);
                                    return;
                                }
                                break;
                            }
                        }

                        nextStep.Add(index);
                    }
                }

                nextStep.ForEach(i => remainingStep.Enqueue(i));
            }
            //Debug.Log("i = " + i);
            //maxDistance = i;
        }

        public void Manhattan(int startPos, List<int> others, int from, ref int[,] distances, BundleTilemapChromosome chrom)
        {
            for (int i = from; i < others.Count; i++) 
            {
                var v1 = chrom.ToMatrixPosition(startPos);
                var v2 = chrom.ToMatrixPosition(others[i]);

                distances[i, from] = distances[from, i] = Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
            }
        }

        public void InitializeDefaultWithContext(List<LBSLayer> contextLayers, Rect selection)
        {
            ContextLayers = new List<LBSLayer>(contextLayers);
            CombinedInteriorLayer = (this as IContextualEvaluator).InteriorLayers(selection);
            InitializeDefault();
        }

        public void InitializeDefault()
        {
            playerCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));
            colliderCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Collider"));

            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Chest")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Axe")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Hammer")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Sword")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Food")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Tree")));
        }

        public object Clone()
        {
            var clone = new DCExploration();

            clone.ContextLayers = new List<LBSLayer>(ContextLayers);
            clone.CombinedInteriorLayer = CombinedInteriorLayer;

            clone.playerCharacteristic = playerCharacteristic;
            clone.colliderCharacteristic = colliderCharacteristic;
            clone.pointsOfInterest = new List<LBSCharacteristic>(pointsOfInterest);
            return clone;
        }
    }
}
