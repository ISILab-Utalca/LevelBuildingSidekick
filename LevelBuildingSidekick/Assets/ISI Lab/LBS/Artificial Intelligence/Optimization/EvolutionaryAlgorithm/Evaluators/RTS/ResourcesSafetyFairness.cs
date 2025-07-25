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
    [System.Serializable]
    public class ResourcesSafetyFairness : IRangedEvaluator
    {
        public float MaxValue => 1;

        public float MinValue => 0;

        public string Tooltip => "Resource Safety Fairness Evaluator";

        [SerializeField, SerializeReference]
        public LBSCharacteristic playerCharacteristc;

        [SerializeField, SerializeReference]
        public List<LBSCharacteristic> resourceCharactersitic = new List<LBSCharacteristic>();

        public float Evaluate(IOptimizable evaluable)
        {
            var chrom = evaluable as BundleTilemapChromosome;

            if (chrom == null)
            {
                throw new Exception("Wrong Chromosome Type");
            }

            float fitness = 0;

            var genes = chrom.GetGenes().Cast<BundleData>().ToList();

            List<int> playersPos = new List<int>();

            for (int i = 0; i < genes.Count; i++)
            {
                if (genes[i] != null)
                {
                    if (genes[i].Characteristics.Contains(playerCharacteristc))
                    {
                        playersPos.Add(i);
                    }
                }
            }

            if (playersPos.Count < 2)
            {
                Debug.LogWarning("Map is not suitable for the evaluation, player count: " + playersPos.Count() + " < 2");
                return MaxValue;
            }

            List<float> localFitness = new List<float>();

            foreach (var pos in playersPos)
            {
                var dist = (int)playersPos.Where(p => p != pos).Min(p => (chrom.ToMatrixPosition(p) - chrom.ToMatrixPosition(pos)).magnitude) / 2;
                float res = 0;

                for (int j = -dist; j <= dist; j++)
                {
                    for (int i = -dist; i <= dist; i++)
                    {
                        var index = chrom.ToIndex(new Vector2(i, j));

                        if (index < 0 || index >= genes.Count || genes[index] == null)
                        {
                            continue;
                        }

                        if (genes[index].Characteristics.Any(c => resourceCharactersitic.Contains(c)))
                        {
                            res++;
                        }
                    }
                }

                localFitness.Add(res);
            }

            if (localFitness.Max() <= 0)
                return MaxValue;

            fitness = localFitness.Min() / localFitness.Max();

            return fitness;
        }

        public object Clone()
        {
            var e = new ResourcesSafetyFairness();
            e.playerCharacteristc = playerCharacteristc;
            e.resourceCharactersitic = new List<LBSCharacteristic>(resourceCharactersitic);
            return e;
        }

        public void InitializeDefault()
        {
            playerCharacteristc = new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Player"));
            resourceCharactersitic.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Tree")));
            resourceCharactersitic.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Food")));
            resourceCharactersitic.Add(new LBSTagsCharacteristic(LBSAssetMacro.GetLBSTag("Rock")));
        }
    }
}