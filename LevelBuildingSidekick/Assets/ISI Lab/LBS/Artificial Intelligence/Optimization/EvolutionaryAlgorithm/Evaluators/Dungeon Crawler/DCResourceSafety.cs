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
    public class DCResourceSafety : IContextualEvaluator, IRangedEvaluator
    {
        public float MaxValue => 1;

        public float MinValue => 0;

        public List<LBSLayer> ContextLayers { get; set; } = new List<LBSLayer>();

        [SerializeField, SerializeReference]
        public LBSCharacteristic playerCharacteristic;

        public List<LBSCharacteristic> resources = new List<LBSCharacteristic>();

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

            List<int> playersInd = new List<int>();
            List<int> resourcesInd = new List<int>();

            for (int i = 0; i < genes.Count; i++)
            {
                if (chrom.IsInvalid(i))
                    continue;
                if (genes[i] != null)
                {
                    if (genes[i].Characteristics.Contains(playerCharacteristic))
                    {
                        playersInd.Add(i);
                        continue;
                    }
                    foreach (var LBSChar in resources)
                    {
                        if (genes[i].Characteristics.Contains(LBSChar))
                        {
                            resourcesInd.Add(i);
                            break;
                        }
                    }
                }
            }

            int bestPossibleScore = (int)(1.25f * resourcesInd.Count);
            int worstPossibleScore = (int)(2.00f * resourcesInd.Count);
            int score = worstPossibleScore;
            if (layer != null)
            {
                if (layer.ID.Equals("Interior"))
                    score = ScoreResourceDistance(playersInd, resourcesInd, chrom, layer.GetModule<SectorizedTileMapModule>());
                else score = ScoreManhattan(playersInd, resourcesInd, chrom);
            }
            else
            {
                score = ScoreManhattan(playersInd, resourcesInd, chrom);
            }

            fitness = Mathf.InverseLerp(worstPossibleScore, bestPossibleScore, score);

            UnityEngine.Assertions.Assert.IsFalse(fitness == float.NaN);
            return fitness;
        }

        private int ScoreResourceDistance(List<int> players, List<int> resources, BundleTilemapChromosome chrom, SectorizedTileMapModule sectorTM)
        {
            var zones = sectorTM.SelectedZones;
            var zonesIndex = zones.Select((z, i) => KeyValuePair.Create(z, i)).ToDictionary(x => x.Key, x => x.Value);
            var zonesDist = sectorTM.ZonesProximity;

            var playerZones = new List<int>();
            for (int i = 0; i < players.Count; i++)
                playerZones.Add(-1);

            for (int i = 0; i < players.Count; i++)
            {
                int p = players[i];
                for(int j = 0; j < zones.Count; j++)
                {
                    if(sectorTM.GetZone(chrom.ToMatrixPosition(p) + Vector2Int.RoundToInt(chrom.Rect.position)).Equals(zones[j]))
                    {
                        playerZones[i] = j;
                        break;
                    }
                }
            }

            int totalScore = 0;

            foreach(int res in resources)
            {
                Zone zone = sectorTM.GetZone(chrom.ToMatrixPosition(res) + Vector2Int.RoundToInt(chrom.Rect.position));
                int rZone = zonesIndex[zone];
                int score = 2;
                foreach(int pZone in playerZones)
                {
                    score = Mathf.Min(score, zonesDist[rZone, pZone]);
                }
                totalScore += score;
            }

            return totalScore;
        }

        private int ScoreManhattan(List<int> players, List<int> resources, BundleTilemapChromosome chrom)
        {
            Debug.LogWarning("El Evaluador Resource Safety no ha implementado un método de evaluación para layers distintas a \"Interior\".");
            return (int)(2.00f * resources.Count);
        }

        public void InitializeDefaultWithContext(List<LBSLayer> contextLayers)
        {
            ContextLayers = new List<LBSLayer>(contextLayers);
            InitializeDefault();
        }

        public void InitializeDefault()
        {
            playerCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));

            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Chest")));
            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Axe")));
            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Hammer")));
            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Sword")));
            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Food")));
            resources.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Tree")));
        }

        public object Clone()
        {
            var clone = new DCResourceSafety();
            clone.ContextLayers = new List<LBSLayer>(ContextLayers);
            clone.playerCharacteristic = playerCharacteristic;
            clone.resources = new List<LBSCharacteristic>(resources);
            return clone;
        }
    }
}
