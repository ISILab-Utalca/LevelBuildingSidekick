using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.LBS.Characteristics;
using ISILab.Macros;
using LBS.Components.TileMap;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    public class SafeAreaFairness : IRangedEvaluator
    {
        public float MaxValue => 1;

        public float MinValue => 0;

        [SerializeField, SerializeReference]
        public LBSCharacteristic playerCharacteristic;

        public float Evaluate(IOptimizable evaluable)
        {
            var chrom = evaluable as BundleTilemapChromosome;

            if (chrom == null)
            {
                throw new Exception("Wrong Chromosome Type");
            }

            float fitness = 0;

            var genes = chrom.GetGenes().Cast<BundleData>().ToList();
            var players = genes.Select((g, i) => new { g, i }).Where(p => p.g != null && p.g.Characteristics.Any(c => c.Equals(playerCharacteristic)));

            if (players.Count() < 2)
            {
                Debug.LogWarning("Map is not suitable for the evaluation, it must have at least 2 players");
            }

            var playersPos = players.Select(x => x.i);


            List<float> localFitness = new List<float>();

            foreach (var pos in playersPos)
            {
                var dist = (int)playersPos.Where(p => p != pos).Min(p => (chrom.ToMatrixPosition(p) - chrom.ToMatrixPosition(pos)).magnitude);

                localFitness.Add(dist);
            }

            fitness = localFitness.Min() / localFitness.Max();

            return fitness;
        }

        public object Clone()
        {
            var e = new SafeAreaFairness();
            e.playerCharacteristic = playerCharacteristic;
            return e;
        }

        public void InitializeDefault()
        {
            playerCharacteristic = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));
            Debug.Log(playerCharacteristic);
        }
    }
}