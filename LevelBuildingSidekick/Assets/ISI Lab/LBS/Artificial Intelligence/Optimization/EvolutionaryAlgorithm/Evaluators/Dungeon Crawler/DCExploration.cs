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
        public float MaxValue => 1;

        public float MinValue => 0;

        public List<LBSLayer> ContextLayers { get; set; } = new List<LBSLayer>();

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

            LBSLayer layer = ContextLayers.FirstOrDefault(l => l.ID.Equals("Interior"));
            
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

            int[,] distances = new int[POIs.Count, POIs.Count];

            if (layer != null)
            {
                for (int i = 0; i < POIs.Count; i++)
                {
                    FloodFill(POIs[i], POIs, i, ref distances, chrom, layer.GetModule<SectorizedTileMapModule>(), layer.GetModule<ConnectedTileMapModule>());
                }
            }
            else
            {
                for (int i = 0; i < POIs.Count; i++)
                {
                    Manhattan(POIs[i], POIs, i, ref distances, chrom);
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

            return sum / (float)score.Count;
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
                    Zone currentZone = tileMap.GetZone(chrom.ToMatrixPosition(current) + Vector2Int.RoundToInt(chrom.Rect.position));
                    //distFromStart[current] = i;
                    remaining.Remove(current);
                    closed.Add(current);

                    var dirs = Directions.Bidimencional.Edges;
                    foreach (var dir in dirs)
                    {
                        var pos = chrom.ToMatrixPosition(current) + dir + Vector2Int.RoundToInt(chrom.Rect.position);

                        int index = chrom.ToIndex(pos - chrom.Rect.position);

                        //if (tileMap.Contains(pos)) // Esto esta mal. Esto es todo el mapa, no solo la seccion seleccionada
                        if (index < 0 || nextStep.Contains(index) || closed.Contains(index) || chrom.IsInvalid(index))
                            continue;

                        Zone otherZone = tileMap.GetZone(pos/* + Vector2Int.RoundToInt(chrom.Rect.position)*/);
                        if (currentZone == null)
                            ;
                        if (!currentZone.Equals(otherZone))
                        {
                            var tile = tileMap.PairTiles.First(tzp => tzp.Tile.Position == pos).Tile;
                            var connection = connectedTM.GetConnections(tile)[dirs.FindIndex(d => d.Equals(-dir))];
                            if (!connection.Equals("Door"))
                                continue;
                            else
                                ;
                        }

                        for (int j = from; j < others.Count; j++)
                        {
                            if (index == others[j])
                            {
                                distances[from, j] = distances[j, from] = i + 1;
                                remainingOthers.Remove(index);
                                if (remaining.Count == 0)
                                    return;
                                break;
                            }
                        }

                        nextStep.Add(index);
                    }
                }

                nextStep.ForEach(i => remainingStep.Enqueue(i));
            }

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

        public void InitializeDefaultWithContext(List<LBSLayer> contextLayers)
        {
            ContextLayers = new List<LBSLayer>(contextLayers);
            InitializeDefault();
        }

        public void InitializeDefault()
        {
            playerCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));
            colliderCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Collider"));

            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Chest")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Axe")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Food")));
            pointsOfInterest.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Tree")));
        }

        public object Clone()
        {
            var clone = new DCExploration();
            clone.ContextLayers = new List<LBSLayer>(ContextLayers);
            clone.playerCharacteristic = playerCharacteristic;
            clone.colliderCharacteristic = colliderCharacteristic;
            clone.pointsOfInterest = new List<LBSCharacteristic>(pointsOfInterest);
            return clone;
        }
    }
}
