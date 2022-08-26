using System;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Schema;
using System.Linq;
using UnityEditor;

namespace LBS.Representation.TileMap
{
    public class TileMapController : Controller
    {
        public TileMapController() : base(LBSController.CurrentLevel.data.GetRepresentation<LBSTileMapData>())
        {
        }

        public override void LoadData()
        {

        }

        public void Optimize()
        {
            var graphData = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            var schemaData = Data as LBSTileMapData;
            var optimized = Utility.HillClimbing.Run(schemaData, graphData,
                            () => { return Utility.HillClimbing.NonSignificantEpochs >= 100; },
                            GetNeighbors,
                            EvaluateMap);
            Debug.Log(optimized);
        }

        private float EvaluateAdjacencies(LBSGraphData graphData, LBSTileMapData schema) // esto podria recivir una funcion de calculo de distancia (?)
        {
            var distValue = 0f;
            foreach (var edge in graphData.edges)
            {
                var r1 = schema.GetRoomByID(edge.firstNode.label);
                var r2 = schema.GetRoomByID(edge.secondNode.label);

                var roomDist = GetRoomDistance(r1, r2);
                if (roomDist <= 1)
                {
                    distValue++;
                }
                else
                {
                    var c = r1.tiles.Count();
                    var max1 = (r1.GetHeight() + r1.GetWidth()) / 2f;
                    var max2 = (r2.GetHeight() + r2.GetWidth()) / 2f;
                    distValue += 1 - (roomDist / (max1 + max2));
                }
            }

            return distValue / (float)graphData.edges.Count;
        }

        private float EvaluateAreas(LBSGraphData graphData, LBSTileMapData tileMap)
        {
            var value = 0f;
            for (int i = 0; i < graphData.nodes.Count; i++)
            {
                var node = graphData.nodes[i];
                var room = tileMap.GetRoomByID(node.label);
                switch (node.room.proportionType)
                {
                    case ProportionType.RATIO:
                        value += EvaluateBtyRatio(node, room);
                        break;
                    case ProportionType.SIZE:
                        value += EvaluateBySize(node, room);
                        break;
                }
            }
            return value / (tileMap.GetRoomAmount() * 1f);
        }

        public float EvaluateMap(LBSTileMapData schemaData, LBSGraphData graphData)
        {
            float alfa = 0.84f;
            float beta = 1 - alfa;
            var adjacenceValue = EvaluateAdjacencies(graphData, schemaData) * alfa;
            var areaValue = EvaluateAreas(graphData, schemaData) * beta;
            return adjacenceValue + areaValue;
        }

        private float EvaluateBtyRatio(LBSNodeData node, RoomData room)
        {
            float current = room.GetRatio();
            float objetive = node.room.xAspectRatio / (float)node.room.yAspectRatio;

            return 1 - (Mathf.Abs(objetive - current) / objetive);
        }

        private float EvaluateBySize(LBSNodeData node, RoomData room)
        {
            var vw = 1f;
            if (room.GetWidth() < node.room.minWidth || room.GetWidth() > node.room.maxWidth)
            {
                var objetive = (node.room.minWidth + node.room.maxWidth) / 2f;
                var current = room.GetWidth();
                vw -= (Mathf.Abs(objetive - current) / objetive);
            }

            var vh = 1f;
            if (room.GetHeight() < node.room.minHeight || room.GetHeight() > node.room.maxHeight)
            {
                var objetive = (node.room.minHeight + node.room.maxHeight) / 2f;
                var current = room.GetHeight();
                vh -= (Mathf.Abs(objetive - current) / objetive);
            }

            return (vw + vh) / 2f;
        }

        private int GetRoomDistance(RoomData r1, RoomData r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var ts1 = r1.tiles;
            var ts2 = r2.tiles;
            for (int i = 0; i < ts1.Count; i++)
            {
                for (int j = 0; j < ts2.Count; j++)
                {
                    var t1 = ts1[i];
                    var t2 = ts2[j];

                    var dist = Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y); // manhattan

                    if (dist <= lessDist)
                    {
                        lessDist = dist;
                    }
                }
            }
            return lessDist;
        }

        public List<LBSTileMapData> GetNeighbors(LBSTileMapData tileMap)
        {
            var neightbours = new List<LBSTileMapData>();

            for (int i = 0; i < tileMap.GetRoomAmount(); i++)
            {
                var room = tileMap.GetRoom(i);
                var vWalls = room.GetVerticalWalls();
                var hWalls = room.GetHorizontalWalls();
                var walls = vWalls.Concat(hWalls);

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSTileMapData;
                    neighbor.SetTiles(wall.allTiles, ""); // setea los tiles a nulo o default
                    neightbours.Add(neighbor);
                }
                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSTileMapData;
                    var tiles = new List<Vector2Int>();
                    wall.allTiles.ForEach(t => tiles.Add(t + wall.dir));
                    neighbor.SetTiles(tiles, room.ID);
                    neightbours.Add(neighbor);
                }
            }
            return neightbours;
        }


        public override void Update()
        {
            //Toolkit.Update();
        }
    }
}

