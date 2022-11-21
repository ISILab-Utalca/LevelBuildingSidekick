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
    public class LBSTileMapController : LBSRepController<LBSSchemaData> , ITileMap
    {
        private static readonly Vector2 tileSize = new Vector2(100, 100); // mover a data

        public float Subdivision => throw new NotImplementedException();

        public float TileSize => tileSize.x;

        public int MatrixWidth => throw new NotImplementedException();

        public LBSTileMapController(LBSGraphView view, LBSSchemaData data) : base(view, data)
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
            var rooms = data.GetRooms();
            var doors = data.GetDoors();
            foreach (var room in rooms)
            {
                foreach (var tile in room.Tiles)
                {
                    var pos = tile.GetPosition();
                    var tv = CreateTileView(tile, pos, tileSize, room);

                    foreach (var door in doors)
                    {
                        var pos1 = door.GetFirstPosition();
                        var pos2 = door.GetSecondPosition();
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

        internal LBSSchemaData RecalculateDoors(LBSSchemaData schema)
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
            Debug.Log("CreateTileView");
            var rs =tile.resolvedStyle;
            var s = tile.style;
            var cs = tile.generateVisualContent += aaa;
            
            tile.SetPosition(new Rect(tilePos * size, size));
            tile.SetSize((int)size.x, (int)size.y);
            tile.SetColor(data.Color);
            tile.SetLabel(tilePos);
            return tile;
        }

        public void aaa(MeshGenerationContext mgc)
        {
            Rect r = new Rect(10,10,10,10);
            float left = 0; float right = r.width; float top = 0; float bottom = r.height;
            Vertex[] k_Vertices = new Vertex[4];
            short[] k_Indices = { 0, 1, 2, 2, 3, 0 };
            Texture2D m_Texture = new Texture2D(10,10);
            k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);
            MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length, m_Texture);
        }

        public LBSSchemaData Optimize()
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

        private float EvaluateAdjacencies(LBSGraphData graphData, LBSSchemaData schema) 
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

        private float EvaluateAreas(LBSGraphData graphData, LBSSchemaData tileMap)
        {
            var value = 0f;
            for (int i = 0; i < graphData.NodeCount(); i++)
            {
                var node = graphData.GetNode(i) as RoomCharacteristicsData;
                var room = tileMap.GetRoom(node.Label);
                switch (node.ProportionType)
                {
                    case ProportionType.RATIO:
                        value += EvaluateByRatio(node, room);
                        break;
                    case ProportionType.SIZE:
                        value += EvaluateBySize(node, room);
                        break;
                }
            }
            return value / (tileMap.RoomCount * 1f);
        }

        public float EvaluateMap(LBSSchemaData schemaData, LBSGraphData graphData)
        {
            var evaluattions = new Tuple<Func<LBSGraphData, LBSSchemaData, float>, float>[]
            {
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(EvaluateAdjacencies,0.5f),
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(EvaluateAreas,0.3f),
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(EvaluateEmptySpace,0.2f)
            };

            var value = 0f;
            for (int i = 0; i < evaluattions.Count(); i++)
            {
                var action = evaluattions[i].Item1;
                var weight = evaluattions[i].Item2;
                value += (float) action?.Invoke(graphData, schemaData) * weight;
            }

            return value;
        }

        private float EvaluateEmptySpace(LBSGraphData graphData, LBSSchemaData schemaData)
        {
            var value = 0f;
            foreach (var room in schemaData.GetRooms())
            {
                var rectArea = room.GetRect().width * room.GetRect().height;
                var tc = room.TilesCount;
                value += 1 - (MathF.Abs(rectArea - tc) / (tc * 1f));
            }

            return value / (schemaData.RoomCount * 1f);
        }

        private float EvaluateByRatio(RoomCharacteristicsData node, RoomData room)
        {
            float current = room.GetRatio();
            float objetive = node.AspectRatio.width / (float)node.AspectRatio.heigth;

            return 1 - (Mathf.Abs(objetive - current) / (float)objetive);
        }

        private float EvaluateBySize(RoomCharacteristicsData node, RoomData room)
        {
            var vw = 1f;
            if (room.GetWidth() < node.RangeWidth.min || room.GetWidth() > node.RangeWidth.max)
            {
                var objetive = node.RangeWidth.Middle;
                var current = room.GetWidth();
                vw -= (Mathf.Abs(objetive - current) / (float)objetive);
            }

            var vh = 1f;
            if (room.GetHeight() < node.RangeHeight.min || room.GetHeight() > node.RangeHeight.max)
            {
                var objetive = node.RangeHeight.Middle;
                var current = room.GetHeight();
                vh -= (Mathf.Abs(objetive - current) / (float)objetive);
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

        public List<LBSSchemaData> GetNeighbors(LBSSchemaData tileMap)
        {
            var neightbours = new List<LBSSchemaData>();

            for (int i = 0; i < tileMap.RoomCount; i++)
            {
                var room = tileMap.GetRoom(i);
                var vWalls = room.GetVerticalWalls();
                var hWalls = room.GetHorizontalWalls();
                var walls = vWalls.Concat(hWalls);

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSSchemaData;
                    var tiles = new List<TileData>();
                    wall.allTiles.ForEach(t => tiles.Add(new TileData( t + wall.dir,room.ID)));
                    neighbor.SetTiles(tiles, room.ID);
                    neightbours.Add(neighbor);
                }

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSSchemaData;
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

