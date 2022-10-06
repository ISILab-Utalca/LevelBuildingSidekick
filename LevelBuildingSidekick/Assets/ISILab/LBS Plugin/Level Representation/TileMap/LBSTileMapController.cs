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
using Random = UnityEngine.Random;

namespace LBS.Representation.TileMap
{
    //schema
    public class LBSTileMapController : LBSRepController<LBSTileMapData> , ITileMap
    {
        private static readonly Vector2 tileSize = new Vector2(100, 100); // mover a data

        public float Subdivision => throw new NotImplementedException();

        public float TileSize => tileSize.x;

        public LBSTileMapController(LBSGraphView view, LBSTileMapData data) : base(view, data)
        {

        }

        public void RemoveTile(TileData tile)
        {
            data.RemoveTile(tile);
        }

        public void AddTile(TileData tile, string id)
        {
            data.AddTile(tile,id);
        }

        public void RemoveDoor(DoorData door)
        {
            data.RemoveDoor(door);
        }

        public void AddDoor(DoorData door)
        {
            if(!data.GetDoors().Contains(door))
                data.AddDoor(door);
        }

        public DoorData GetDoor(TileData t1,TileData t2)
        {
            var temp = new DoorData(t1.GetRoomID(), t2.GetRoomID(),t1.GetPosition(), t2.GetPosition());
            foreach ( var door in data.GetDoors())
            {
                if(door.Equals(temp))
                {
                    return (door);
                }
            }
            return null;
        }

        public override void OnContextualBuid( MainView view, ContextualMenuPopulateEvent cmpe)
        {
            cmpe.menu.AppendAction("TileMap/Optimizar", (dma) => {
                view.DeleteElements(elements);
                LBSController.CurrentLevel.data.AddRepresentation(Optimize());
                PopulateView(view);
            });
        }

        public override void PopulateView(MainView view)
        {
            var dt = data.GetDT();
            var rooms = data.GetRooms();
            var doors = data.GetDoors();
            foreach (var room in rooms)
            {
                foreach (var tile in room.Tiles)
                {
                    var pos = tile.GetPosition();// - dt;
                    var tv = CreateTileView(tile, pos, tileSize, room);

                    foreach (var door in doors)
                    {
                        var pos1 = door.GetFirstPosition();// - dt;
                        var pos2 = door.GetSecondPosition();// - dt;
                        if (pos == pos1)
                            tv.ShowDir(pos2 - pos1);
                        else if (pos == pos2)
                            tv.ShowDir(pos1 - pos2);
                    }

                    elements.Add(tv);
                    view.AddElement(tv);
                }
            }

            
        }

        /*
        public override void PopulateView(MainView view) 
        {
            // Esto demora 1.8 seg en completarse con alrededor de 550 tiles,
            // es necesario mejorar la eficinecia en este paso ya que añade mucha demora.
            // Se sugiere probar con object pool o algo asi. (!!!)
            var mtx = data.GetMatrix();
            var dt = -data.GetRect().min;
            for (int i = 0; i < mtx.GetLength(0); i++)
            {
                for (int j = 0; j < mtx.GetLength(1); j++)
                {
                    var roomId = mtx[i, j];
                    var roomData = data.GetRoom(roomId);

                    if (roomData == null)
                        continue;

                    var pos = new Vector2Int(i, j);
                    var posT = new Vector2Int(i, j) - dt;
                    var t = roomData.GetTile(posT);
                    var tv = CreateTileView(t, pos, tileSize, roomData); // esta es la linea en cuestion que lagea (!!!)

                    var doors = data.GetDoors();
                    foreach (var d in doors)
                    {
                        var pos1 = d.GetFirstPosition() + dt;
                        var pos2 = d.GetSecondPosition() + dt;
                        if(pos == pos1)
                        {
                            tv.ShowDir(pos2 - pos1);
                        }
                        else if(pos == pos2)
                        {
                            tv.ShowDir(pos1 - pos2);
                            //tv.Add(new DoorView(pos2, pos1, roomData.Color));
                        }
                    }

                    elements.Add(tv);
                    view.AddElement(tv);
                }
            }
        }
        */
        internal LBSTileMapData RecalculateDoors(LBSTileMapData schema)
        {
            var graphData = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            schema.ClearDoors();
            var edges = graphData.GetEdges();
            foreach (var e in edges)
            {
                var room1 = schema.GetRoom(e.FirstNodeLabel);
                var room2 = schema.GetRoom(e.SecondNodeLabel);
                var tiles = GetNearestTiles(room1,room2);

                var doorTiles = tiles[Random.Range(0,tiles.Count)];
                var door = new DoorData(room1.ID, room2.ID, doorTiles.Item1.GetPosition(), doorTiles.Item2.GetPosition());
                schema.AddDoor(door);
            }
            return schema;
        }

        private List<Tuple<TileData,TileData>> GetNearestTiles(RoomData r1,RoomData r2)
        {
            var lessDist = int.MaxValue;
            var nearest = new List<Tuple<TileData, TileData>>();
            var ts1 = r1.Tiles;
            var ts2 = r2.Tiles;
            for (int i = 0; i < ts1.Count; i++)
            {
                for (int j = 0; j < ts2.Count; j++)
                {
                    var t1 = ts1[i].GetPosition();
                    var t2 = ts2[j].GetPosition();

                    var dist = Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y); // manhattan

                    if (dist == lessDist)
                    {
                        nearest.Add(new Tuple<TileData,TileData>(ts1[i], ts2[j]));
                    }
                    else if(dist < lessDist)
                    {
                        nearest.Clear();
                        nearest.Add(new Tuple<TileData, TileData>(ts1[i], ts2[j]));
                        lessDist = dist;
                    }
                }
            }
            return nearest;
        }

        public override string GetName()
        {
            return "Schema Layer";
        }

        private LBSTileView CreateTileView(TileData tileData,Vector2Int tilePos, Vector2 size, RoomData data)
        {
            var tile = new LBSTileView(tileData,view);
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
            optimized.RecalculateTilePos();
            LBSController.CurrentLevel.data.AddRepresentation(optimized);
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
                    wall.allTiles.ForEach(t => tiles.Add(new TileData( t + wall.dir,room.ID)));
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

        public Vector2Int ToTileCoords(Vector2 pos)
        {
            int x = (pos.x > 0)? (int)(pos.x / (float)TileSize): (int)(pos.x / (float)TileSize) - 1;
            int y = (pos.y > 0)? (int)(pos.y / (float)TileSize): (int)(pos.y / (float)TileSize) - 1;

            return new Vector2Int(x, y);
        }

        public Vector2 FromTileCoords(Vector2 position)
        {
            return position * TileSize;
        }
    }
}

