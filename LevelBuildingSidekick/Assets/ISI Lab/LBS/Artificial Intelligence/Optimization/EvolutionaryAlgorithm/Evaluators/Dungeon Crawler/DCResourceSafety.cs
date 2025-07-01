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
            throw new NotImplementedException();
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
