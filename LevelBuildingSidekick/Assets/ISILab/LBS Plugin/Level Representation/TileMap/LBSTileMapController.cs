using System;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using LBS.Graph;
using LBS.Schema;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using LBS.ElementView;
using UnityEngine.UIElements;

namespace LBS.Representation.TileMap
{
    public class LBSTileMapController : LBSRepController<LBSTileMapData>
    {
        private static readonly Vector2 tileSize = new Vector2(100, 100);

        public LBSTileMapController(GraphView view, LBSTileMapData data) : base(view, data)
        {

        }

        public override void OnContextualBuid( MainView view, ContextualMenuPopulateEvent cmpe)
        {
            cmpe.menu.AppendAction("TileMap/TEST", (dma) => { Debug.Log("test tilemap"); });

            cmpe.menu.AppendAction("TileMap/Optimizar", (dma) => {
                view.DeleteElements(elements);
                LBSController.CurrentLevel.data.AddRepresentation(Optimize());
                PopulateView(view);
            });
        }

        public override void PopulateView(MainView view)
        {
            // Esto demora 1.8 seg en completarse con alrededor de 550 tiles,
            // es necesario mejorar la eficinecia en este paso ya que añade mucha demora.
            // Se sugiere probar con object pool o algo asi. (!!!)
            var mtx = data.GetMatrix();
            for (int i = 0; i < mtx.GetLength(0); i++)
            {
                for (int j = 0; j < mtx.GetLength(1); j++)
                {
                    var roomId = mtx[i, j];
                    var roomData = data.GetRoom(roomId);
                    if (roomData != null)
                    {
                        var tv = CreateTileView(new Vector2Int(i, j), tileSize, roomData); // esta es la linea en cuestion que lagea (!!!)
                        elements.Add(tv);
                        view.AddElement(tv);
                    }
                }
            }
        }
        public override string GetName()
        {
            return "Schema Layer";
        }

        private TileView CreateTileView(Vector2Int tilePos, Vector2 size, RoomData data)
        {
            var tile = new TileView();
            tile.SetPosition(new Rect(tilePos * size, size));
            tile.SetSize((int)size.x, (int)size.y);
            tile.SetColor(data.Color);
            tile.SetLabel(tilePos);
            return tile;
        }

        public LBSTileMapData Optimize()
        {
            var graphData = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            var schemaData = data;
            var optimized = Utility.HillClimbing.Run(schemaData, graphData,
                            () => { return Utility.HillClimbing.NonSignificantEpochs >= 100; },
                            GetNeighbors,
                            EvaluateMap);
            return optimized;
        }

        private float EvaluateAdjacencies(LBSGraphData graphData, LBSTileMapData schema) 
        {
            if (graphData.EdgeCount() <= 0)
            {
                Debug.LogWarning("Cannot calculate the adjacency of a map are nodes that are not connected.");
                return 0;
            }

            var distValue = 0f;
            for (int i = 0; i < graphData.EdgeCount(); i++)
            {
                var edge = graphData.GetEdge(i);
           
                var r1 = schema.GetRoom(edge.FirstNodeLabel);
                var r2 = schema.GetRoom(edge.SecondNodeLabel);

                var roomDist = GetRoomDistance(r1, r2);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija (?)
                if (roomDist <= 1)
                {
                    distValue++;
                }
                else
                {
                    var c = r1.TilesCount;
                    var max1 = (r1.GetHeight() + r1.GetWidth()) / 2f;
                    var max2 = (r2.GetHeight() + r2.GetWidth()) / 2f;
                    distValue += 1 - (roomDist / (max1 + max2));
                }
            }

            return distValue / (float)graphData.EdgeCount();
        }

        private float EvaluateAreas(LBSGraphData graphData, LBSTileMapData tileMap)
        {
            var value = 0f;
            for (int i = 0; i < graphData.NodeCount(); i++)
            {
                var node = graphData.GetNode(i) as RoomCharacteristicsData;
                var room = tileMap.GetRoom(node.Label);
                switch (node.ProportionType)
                {
                    case ProportionType.RATIO:
                        value += EvaluateBtyRatio(node, room);
                        break;
                    case ProportionType.SIZE:
                        value += EvaluateBySize(node, room);
                        break;
                }
            }
            return value / (tileMap.RoomCount * 1f);
        }

        public float EvaluateMap(LBSTileMapData schemaData, LBSGraphData graphData)
        {
            float alfa = 0.84f;
            float beta = 1 - alfa;
            var adjacenceValue = EvaluateAdjacencies(graphData, schemaData) * alfa;
            var areaValue = EvaluateAreas(graphData, schemaData) * beta;
            return adjacenceValue + areaValue;
        }

        private float EvaluateBtyRatio(RoomCharacteristicsData node, RoomData room)
        {
            float current = room.GetRatio();
            float objetive = node.AspectRatio.width / (float)node.AspectRatio.heigth;

            return 1 - (Mathf.Abs(objetive - current) / objetive);
        }

        private float EvaluateBySize(RoomCharacteristicsData node, RoomData room)
        {
            var vw = 1f;
            if (room.GetWidth() < node.RangeWidth.min || room.GetWidth() > node.RangeWidth.max)
            {
                var objetive = node.RangeWidth.Middle;
                var current = room.GetWidth();
                vw -= (Mathf.Abs(objetive - current) / objetive);
            }

            var vh = 1f;
            if (room.GetHeight() < node.RangeHeight.min || room.GetHeight() > node.RangeHeight.max)
            {
                var objetive = node.RangeHeight.Middle;
                var current = room.GetHeight();
                vh -= (Mathf.Abs(objetive - current) / objetive);
            }

            return (vw + vh) / 2f;
        }

        private int GetRoomDistance(RoomData r1, RoomData r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var ts1 = r1.Tiles;
            var ts2 = r2.Tiles;
            for (int i = 0; i < ts1.Count; i++)
            {
                for (int j = 0; j < ts2.Count; j++)
                {
                    var t1 = ts1[i].GetPosition();
                    var t2 = ts2[j].GetPosition();

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

            for (int i = 0; i < tileMap.RoomCount; i++)
            {
                var room = tileMap.GetRoom(i);
                var vWalls = room.GetVerticalWalls();
                var hWalls = room.GetHorizontalWalls();
                var walls = vWalls.Concat(hWalls);

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSTileMapData;
                    var tiles = new List<TileData>();
                    wall.allTiles.ForEach(t => tiles.Add(new TileData( t + wall.dir)));
                    neighbor.SetTiles(tiles, room.ID);
                    neightbours.Add(neighbor);
                }

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSTileMapData;
                    neighbor.RemoveTiles(wall.allTiles);
                    neightbours.Add(neighbor);
                }
                
            }
            return neightbours;
        }

    }
}

