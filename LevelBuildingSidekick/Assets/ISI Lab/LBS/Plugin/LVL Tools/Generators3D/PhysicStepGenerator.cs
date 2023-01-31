using LBS.Components;
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
    public enum PivotType // (!!!) PyshicStep esto tiene que estar en otro lado
    {
        Center,
        Edge
    }

    public class PhysicStepGenerator : Generator3D
    {
        

        private static List<Vector2Int> dirs = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private LBSSchemaData schema;
        //private LBSGraphData graph = null;
        private float tileSize = 1f;
        
        public override GameObject Generate(LBSLayer layer)
        {
            if(schema == null/* || graph == null*/)
            {
                Debug.LogWarning("cannot be generated, there is no information about the map to load.");
                return null;
            }

            var mainPivot = new GameObject("New level 3D");
            /*foreach (var node in graph.GetNodes())
            {
                var bNames = (node as RoomCharacteristicsData).bundlesNames;
                var bundle = RoomElementBundle.Combine(bNames.Select(n => DirectoryTools.GetScriptable<RoomElementBundle>(n)).ToList());

                var room = schema.GetRoom(node.Label);
                var doors = schema.GetDoors();
                foreach (var tp in room.TilesPositions)
                {
                    var pivot = new GameObject();
                    pivot.transform.SetParent(mainPivot.transform);
                    pivot.transform.position = new Vector3(tp.x, 0, -tp.y) * (tileSize);

                    var cBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Center).ToList();
                    var eBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Edge).ToList();

                    foreach (var bundel in cBundle)
                    {
                        GenPhysicCenter(bundel,pivot.transform);
                    }
                    var tile = schema.GetTile(tp);
                    GenPhysicEdge(schema,eBundle, pivot.transform, tile);
                    //GenPhysicCorners(blabla);
                }
            }*/
            return mainPivot;
        }

        private GameObject GenPhysicCenter(ItemCategory bundle, Transform parent)
        {
            var prefs = bundle.items;
            var r = SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);

            if(false) // (!) dejar esta como variable que el usuario pueda controlar, y cambiar a que funcione con la rotacion del tile
                r.transform.Rotate(new Vector3(0, 90, 0) * Random.Range(0, 4));

            return r;
        }

        private void InstantiateEdge(ItemCategory bundle,Transform pivot,Vector2 dir)
        {
            var prefs = bundle.items;
            var go = SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], pivot);
            //var wThic = LBSController.CurrentLevel.data.WallThickness;
            //var tSize = LBSController.CurrentLevel.data.TileSize;
            //var mag = (wThic + tSize) * 0.5f;
            dir = dir.normalized;
            //var d = new Vector3(dir.x * mag, 0, -dir.y * mag);
            //go.transform.position += d;
            var a = go.transform.position - pivot.transform.position;
            go.transform.LookAt(a + go.transform.position,Vector3.up);
        }

        private GameObject GenPhysicEdge(LBSSchemaData schema,List<ItemCategory> bundle, Transform parent, TileData tile)//,LBSTileMapData schema)
        {
            var bWall = bundle.Where(b => b.category == "Wall").ToList();
            var bDoor = bundle.Where(b => b.category == "Door").ToList();

            var doors = schema.GetDoors();
            var room = schema.GetRoom(tile.Position);
            for (int i = 0; i < dirs.Count; i++)
            {
                var other = schema.GetTile(tile.Position + dirs[i]);
                
                if (other == null) // si no hya otro tile pone muralla
                {
                    InstantiateEdge(bWall[Random.Range(0, bWall.Count)], parent, dirs[i]);
                    continue;
                }

                var tempDoor = new DoorData(tile.Position, tile.Position + dirs[i]);
                if (doors.Contains(tempDoor)) // si es una puerta pone puerta
                {
                    InstantiateEdge(bDoor[Random.Range(0, bDoor.Count)], parent, dirs[i]);
                    continue;
                }

                var otherRoom = schema.GetRoom(other.Position);
                if(!otherRoom.Equals(room)) // si son de diferentes habitaciones pone muralla
                {
                    InstantiateEdge(bWall[Random.Range(0, bWall.Count)], parent, dirs[i]);
                } // si son de la misma no pone nada
            }
            return null;
        }   

        public void Init(LBSLevelData levelData)
        {
            //this.schema = levelData.GetRepresentation<LBSSchemaData>();
            //this.graph = levelData.GetRepresentation<LBSGraphData>();
            //tileSize = levelData.TileSize;
        }

        public override void Init(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }
    }
}