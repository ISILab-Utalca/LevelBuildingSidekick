using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using LBS.Bundles;
using LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
    [Serializable]
    public class DataStealth : BaseQuestNodeData
    {
        [SerializeField]
        public Vector2Int objective = Vector2Int.zero;
        /// <summary>
        /// Objects with a default trigger that will stop catch the player
        /// </summary>
        [SerializeField]
        public List<BundleGraph> bundlesObservers;
        
        private readonly HashSet<Bundle.EElementFlag> requiredObserverTags = new()
        {
            Bundle.EElementFlag.Enemy
        }; 
        
        public DataStealth(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
            iconGuid = FoeIcon;
            color = LBSSettings.Instance.view.colorStealth;
            bundlesObservers = new List<BundleGraph>();
        }
        
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataStealth stealthData) return;
            objective = stealthData.objective;
            bundlesObservers = new List<BundleGraph>(stealthData.bundlesObservers);
        }
        
        public override List<string> ReferencedLayerNames()
        {
            return bundlesObservers.Select(bundleGraph => bundleGraph.GetLayerName()).ToList();
        }
        
        public override void Resize()
        {
            ResizeToFitBundles(bundlesObservers);
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var stealthOther = other as DataStealth;
            if(stealthOther == null) return false;

            HashSet<BundleGraph> observersHash = new HashSet<BundleGraph>();
            foreach (var observer in bundlesObservers)
            {
                observersHash.Add(observer);
            }
            HashSet<BundleGraph> otherObserversHash = new HashSet<BundleGraph>();
            foreach (var observer in stealthOther.bundlesObservers)
            {
                otherObserversHash.Add(observer);
            }
            
            if (observersHash.Count != otherObserversHash.Count) return false;
            
            return observersHash.SetEquals(otherObserversHash);
        }

        public override bool IsValid()
        {
            return bundlesObservers.Any();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            bundlesObservers.Clear();
            
            TrySetBundleGraphList(layers, tiles,  ref bundlesObservers, requiredObserverTags);

            if (bundlesObservers.Count == 0)
            {
                objective = Vector2Int.zero;
                return;
            }

            // Area
            int xMin = int.MaxValue, xMax = int.MinValue;
            int yMin = int.MaxValue, yMax = int.MinValue;

            foreach (var bundleGraph in bundlesObservers)
            {
                if (bundleGraph.Area.xMin < xMin) xMin = (int)bundleGraph.Area.xMin;
                if (bundleGraph.Area.xMax > xMax) xMax = (int)bundleGraph.Area.xMax;
                if (bundleGraph.Area.yMin < yMin) yMin = (int)bundleGraph.Area.yMin;
                if (bundleGraph.Area.yMax > yMax) yMax = (int)bundleGraph.Area.yMax;
            }

            // Generate all tiles in bounding rect
            HashSet<Vector2Int> allTiles = new HashSet<Vector2Int>();
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    allTiles.Add(new Vector2Int(i, j));
                }
            }

            // Mark busy tiles
            HashSet<Vector2Int> busyTiles = new HashSet<Vector2Int>();
            foreach (var observerBG in bundlesObservers)
            {
                for (int i = (int)observerBG.Area.xMin; i < (int)observerBG.Area.xMax; i++)
                {
                    for (int j = (int)observerBG.Area.yMin; j < (int)observerBG.Area.yMax; j++)
                    {
                        busyTiles.Add(new Vector2Int(i, j));
                    }
                }
            }

            // Free tiles = allTiles - busyTiles
            allTiles.ExceptWith(busyTiles);

            // Pick an objective
            objective = allTiles.Any() ? 
                allTiles.ElementAt(UnityEngine.Random.Range(0, allTiles.Count)) : Vector2Int.zero;
          
        }
    }
}