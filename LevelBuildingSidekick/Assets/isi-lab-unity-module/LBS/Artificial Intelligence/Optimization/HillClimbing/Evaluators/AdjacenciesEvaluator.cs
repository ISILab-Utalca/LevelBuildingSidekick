using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.AI.Wrappers;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //Debug.Log(Tooltip);

            List<LBSModule> modules = (evaluable as OptimizableModules).Modules;
            var connectedZones = modules.GetModule<ConnectedZonesModule>();

            int edgeCount = connectedZones.Edges.Count;
            if (edgeCount <= 0)
            {
                // LOGS MOVED TO HILL CLIMBING ASSISTANT EDITOR
                //Debug.Log("Cannot calculate the adjacency of a map are nodes that are not connected.");
                return 1;
            }

            var zones = modules.GetModule<SectorizedTileMapModule>();
            if (zones.ZonesWithTiles.Count <= 0)
            {
                //Debug.Log("[ISI Lab]: the schema you are trying to evaluate does not have areas.");
                return 0;
            }

            float distValue = 0f;
            for (int i = 0; i < edgeCount; i++)
            {
                ZoneEdge edge = connectedZones.Edges[i];

                List<LBSTile> r1 = zones.GetTiles(edge.First);
                if (r1.Count < 1) continue;
                List<LBSTile> r2 = zones.GetTiles(edge.Second);
                if (r2.Count < 1) continue;

                //Rect bounds1 = r1.GetBounds();
                //Rect bounds2 = r2.GetBounds();

                float roomDist = float.MaxValue;
                //if(zones.IsRectangular(edge.First, bounds1, r1) && zones.IsRectangular(edge.Second, bounds2, r2))
                //{
                //    float lessDist = float.MaxValue;
                //    float dx = Mathf.Max(0, Mathf.Max(bounds1.xMin - bounds2.xMax, bounds2.xMin - bounds1.xMax));
                //    float dy = Mathf.Max(0, Mathf.Max(bounds1.yMin - bounds2.yMax, bounds2.yMin - bounds1.yMax));
                //    if (dx == 0 && dy == 0) lessDist = 0;
                //    else if (dx == 0) lessDist = dy;
                //    else if (dy == 0) lessDist = dx;
                //    else lessDist = dx * dx + dy * dy;
                //    roomDist = lessDist+1;
                //    //Debug.Log("Bounds distance");
                //}
                //else
                //{
                //}
                    roomDist = zones.GetRoomDistance(edge.First, edge.Second, r1, r2); // TODO: Make it receive a distance calculation function.
                    //Debug.Log("Tile per tile distance");

                distValue += 1 / roomDist;
            }

            if (edgeCount <= 0)
            {
                return 0;
            }

            //List<LBSTile> tiles = modules.GetModule<TileMapModule>().Tiles;

            return distValue / edgeCount;
        }

        public void InitializeDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}