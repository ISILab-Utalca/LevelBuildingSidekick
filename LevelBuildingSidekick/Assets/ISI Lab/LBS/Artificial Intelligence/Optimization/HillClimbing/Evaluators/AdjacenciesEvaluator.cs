﻿using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.AI.Wrappers;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization
{
    public class AdjacenciesEvaluator : IEvaluator
    {
        private SectorizedTileMapModule zones;
        private ConnectedZonesModule connectedZones;

        public AdjacenciesEvaluator() { }

        public AdjacenciesEvaluator(LBSLayer layer)
        {
            this.zones = layer.GetModule<SectorizedTileMapModule>();
            this.connectedZones = layer.GetModule<ConnectedZonesModule>();
        }

        public string Tooltip => "Adjacencies Evaluator";

        public object Clone()
        {
            throw new System.NotImplementedException();
        }

        public float Evaluate(IOptimizable evaluable)
        {
            var layer = (evaluable as OptimizableModules).Modules;
            var connectedZones = layer.GetModule<ConnectedZonesModule>();
            var zones = layer.GetModule<SectorizedTileMapModule>();

            var edgeCount = connectedZones.Edges.Count;
            if (edgeCount <= 0)
            {
                Debug.Log("Cannot calculate the adjacency of a map are nodes that are not connected.");
                return 1;
            }

            if (zones.ZonesWithTiles.Count <= 0)
            {
                Debug.Log("[ISI Lab]: the schema you are trying to evaluate does not have areas.");
                return 0;
            }

            float distValue = 0f;
            for (int i = 0; i < edgeCount; i++)
            {
                var edge = connectedZones.Edges[i];

                var r1 = zones.GetTiles(edge.First);
                var r2 = zones.GetTiles(edge.Second);

                if (r1.Count < 1 || r2.Count < 1)
                    continue;

                float roomDist = zones.GetRoomDistance(edge.First, edge.Second); // TODO: Make it receive a distance calculation function.

                distValue += 1 / roomDist;
            }

            if (edgeCount <= 0)
            {
                return 0;
            }

            var tiles = layer.GetModule<TileMapModule>().Tiles;

            return distValue / edgeCount;
        }

        public void InitializeDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}