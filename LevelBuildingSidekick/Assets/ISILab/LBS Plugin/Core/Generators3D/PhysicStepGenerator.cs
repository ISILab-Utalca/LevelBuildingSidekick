using LBS.Graph;
using LBS.Representation;
using LBS.Representation.TileMap;
using LBS.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class PhysicStepGenerator : Generator
    {
        private static List<Vector2Int> dirs = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private LBSTileMapData schema;
        private LBSGraphData graph;
        private float tileSize = 1f;
        
        public override GameObject Generate()
        {
            if(schema == null || graph == null)
            {
                Debug.LogWarning("cannot be generated, there is no information about the map to load.");
                return null;
            }

            var mainPivot = new GameObject("New level 3D");
            foreach (var node in graph.GetNodes())
            {
                var bNames = (node as RoomCharacteristicsData).bundlesNames;
                var bundle = RoomElementBundle.Combine(bNames.Select(n => DirectoryTools.GetScriptable<RoomElementBundle>(n)).ToList());

                var room = schema.GetRoom(node.Label);
                var doors = schema.GetDoors();
                foreach (var tile in room.Tiles)
                {
                    var pivot = new GameObject();
                    pivot.transform.SetParent(mainPivot.transform);
                    //var tSize = LBSController.CurrentLevel.data.TileSize;
                    pivot.transform.position = new Vector3(tile.GetPosition().x, 0, tile.GetPosition().y) * (tileSize); // (* vector de tamaño de tile en mundo) (!)

                    var cBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Center).ToList();
                    var eBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Edge).ToList();

                    foreach (var bundel in cBundle)
                    {
                        GenPhysicCenter(bundel,pivot.transform);
                    }
                    GenPhysicEdge(eBundle, pivot.transform, tile);
                }
            }
            return mainPivot;
        }

        private GameObject GenPhysicCenter(ItemCategory bundle, Transform parent)
        {
            var prefs = bundle.items;
            var r = SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);

            if(false) // dejar esta como variable que el usuario pueda controlar
                r.transform.Rotate(new Vector3(0, 90, 0) * Random.Range(0, 4));

            return r;
        }

        private void InstantiateEdge(ItemCategory bundle,Transform pivot,Vector2 dir)
        {
            var prefs = bundle.items;
            var go = SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], pivot);
            var wThic = LBSController.CurrentLevel.data.WallThickness;
            var tSize = LBSController.CurrentLevel.data.TileSize;
            var mag = (wThic + tSize) * 0.5f;
            dir = dir.normalized;
            var d = new Vector3(dir.x * mag, 0, dir.y * mag);
            go.transform.position += d;
            var a = go.transform.position - pivot.transform.position;
            go.transform.LookAt(a + go.transform.position,Vector3.up);
        }

        private GameObject GenPhysicEdge(List<ItemCategory> bundle, Transform parent, TileData tile)//,LBSTileMapData schema)
        {
            var bWall = bundle.Where(b => b.category == "Wall").ToList();
            var bDoor = bundle.Where(b => b.category == "Door").ToList();

            var doors = schema.GetDoors();
            for (int i = 0; i < dirs.Count; i++)
            {
                var other = schema.GetTile(tile.GetPosition() + dirs[i]);
                
                if (other == null) // si no hya otro tile pone muralla
                {
                    InstantiateEdge(bWall[Random.Range(0, bWall.Count)], parent, dirs[i]);
                    continue;
                }

                var tempDoor = new DoorData("", "", tile.GetPosition(), tile.GetPosition() + dirs[i]);
                if (doors.Contains(tempDoor)) // si es una puerta pone puerta
                {
                    InstantiateEdge(bDoor[Random.Range(0, bDoor.Count)], parent, dirs[i]);
                    continue;
                }

                if(!other.GetRoomID().Equals(tile.GetRoomID())) // si son de diferentes habitaciones pone muralla
                {
                    InstantiateEdge(bWall[Random.Range(0, bWall.Count)], parent, dirs[i]);
                } // si son de la misma no pone nada
            }

            return null;
        }   

        public override void Init(LevelData levelData)
        {
            this.schema = levelData.GetRepresentation<LBSTileMapData>();
            this.graph = levelData.GetRepresentation<LBSGraphData>();
            tileSize = levelData.TileSize;
        }
    }
}