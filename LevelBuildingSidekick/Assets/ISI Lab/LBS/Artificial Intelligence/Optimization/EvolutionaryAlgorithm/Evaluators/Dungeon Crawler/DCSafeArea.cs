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
    public class DCSafeArea : IContextualEvaluator, IRangedEvaluator
    {
        public float MaxValue => 1;

        public float MinValue => 0;

        public List<LBSLayer> ContextLayers { get; set; } = new List<LBSLayer>();

        [SerializeField, SerializeReference]
        public LBSCharacteristic playerCharacteristic;

        [SerializeField, SerializeReference]
        public LBSCharacteristic enemiesCharacteristic;

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
            List<int> enemiesInd = new List<int>();

            for (int i = 0; i < genes.Count; i++)
            {
                if (chrom.IsInvalid(i))
                    continue;
                if (genes[i] != null)
                {
                    var geneChars = genes[i].Characteristics;
                    if (geneChars.Contains(playerCharacteristic))
                    {
                        playersInd.Add(i);
                    }
                    else if(geneChars.Contains(enemiesCharacteristic))
                    {
                        enemiesInd.Add(i);
                    }
                }
            }

            int bestPossibleScore = (int)(2.00f * enemiesInd.Count);
            int worstPossibleScore = (int)(1.00f * enemiesInd.Count);
            int score = worstPossibleScore;
            if(layer != null)
            {
                if (layer.ID.Equals("Interior"))
                    score = ScoreEnemyDistance(playersInd, enemiesInd, chrom, layer.GetModule<SectorizedTileMapModule>());
                else score = ScoreManhattan(playersInd, enemiesInd, chrom);
            }
            else
            {
                score = ScoreManhattan(playersInd, enemiesInd, chrom);
            }

            fitness = Mathf.InverseLerp(worstPossibleScore, bestPossibleScore, score);

            UnityEngine.Assertions.Assert.IsFalse(fitness == float.NaN);
            return fitness;
        }
        // Como es practicamente a igual a DCResourceSafety.ScoreResourceDistance, podria ser una extension a futuro?
        private int ScoreEnemyDistance(List<int> players, List<int> enemies, BundleTilemapChromosome chrom, SectorizedTileMapModule sectorTM)
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
                for (int j = 0; j < zones.Count; j++)
                {
                    if (sectorTM.GetZone(chrom.ToMatrixPosition(p) + Vector2Int.RoundToInt(chrom.Rect.position)).Equals(zones[j]))
                    {
                        playerZones[i] = j;
                        break;
                    }
                }
            }

            int totalScore = 0;

            foreach(int enemy in enemies)
            {
                Zone zone = sectorTM.GetZone(chrom.ToMatrixPosition(enemy) + Vector2Int.RoundToInt(chrom.Rect.position));
                int eZone = zonesIndex[zone];
                int score = 2;
                foreach (int pZone in playerZones)
                {
                    score = Mathf.Min(score, zonesDist[eZone, pZone]);
                }
                totalScore += score;
            }

            return totalScore;
        }

        private int ScoreManhattan(List<int> players, List<int> enemies, BundleTilemapChromosome chrom)
        {
            int totalScore = 0;

            int maxSignificantDist = (int)(Mathf.Max(chrom.Rect.width, chrom.Rect.height) * 0.25f);
            int halfDist = maxSignificantDist / 2;
            foreach(int e in enemies)
            {
                int score = 2;
                Vector2Int ePos = chrom.ToMatrixPosition(e);
                foreach(int p in players)
                {
                    Vector2Int pPos = chrom.ToMatrixPosition(p);
                    int dist = Mathf.Abs(ePos.x - pPos.x) + Mathf.Abs(ePos.y - pPos.y);
                    score = Mathf.Min(score, dist / halfDist);
                }
                totalScore += score;
            }

            return totalScore;

            //Debug.LogWarning("El Evaluador Safe Area no ha implementado un método de evaluación para layers distintas a \"Interior\".");
            //return (int)(1.00f * enemies.Count);
        }

        public void InitializeDefaultWithContext(List<LBSLayer> contextLayers)
        {
            ContextLayers = new List<LBSLayer>(contextLayers);
            InitializeDefault();
        }

        public void InitializeDefault()
        {
            playerCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));
            enemiesCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Enemies"));
        }

        public object Clone()
        {
            var clone = new DCSafeArea();
            clone.ContextLayers = new List<LBSLayer>(ContextLayers);
            clone.playerCharacteristic = playerCharacteristic;
            clone.enemiesCharacteristic = enemiesCharacteristic;
            return clone;
        }
    }
}