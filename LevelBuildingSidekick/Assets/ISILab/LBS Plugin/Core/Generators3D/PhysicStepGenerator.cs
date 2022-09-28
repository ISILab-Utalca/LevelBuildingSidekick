using LBS.Graph;
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
        
        public override GameObject Generate()
        {
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
                    pivot.transform.position = new Vector3(tile.GetPosition().x, 0, tile.GetPosition().y); // (* vector de tamaño de tile en mundo) (!)

                    foreach (var cat in bundle.GetCategories())
                    {
                        GameObject go;
                        switch (cat.pivotType)
                        {
                            case PivotType.Center:
                                go = GenPhysicCenter(cat, pivot.transform);
                                break;
                            case PivotType.Edge:
                                for (int i = 0; i < dirs.Count; i++) // esto podria cambiar si hay tiles hexagonales o triangulares (?)
                                {
                                    var d = new DoorData("","",tile.GetPosition(), tile.GetPosition() + dirs[i]);
                                    if(doors.Contains(d)) // En esta direccion hay una puerta
                                    {

                                    }
                                    else // Si no hay puerta hay muralla
                                    {

                                    }
                                    go = GenPhysicEdge(cat, pivot.transform,dirs[i]); // implementar construccion de murrallas y puertas (!!!)
                                }
                                break;
                        }
                    }

                }
            }
            return mainPivot;
        }

        private GameObject GenPhysicCenter(ItemCategory bundle, Transform parent)
        {
            var prefs = bundle.items;
            return SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);
        }

        private GameObject GenPhysicEdge(ItemCategory bundle, Transform parent, Vector2 dir)
        {
            var x = DirectoryTools.GetScriptable<Tags_SO>();


            var prefs = bundle.items;
            SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);

            return null;
            //var r = SceneView.Instantiate();
            //return r;
        }   

        public override void Init(LevelData levelData)
        {
            this.schema = levelData.GetRepresentation<LBSTileMapData>();
            this.graph = levelData.GetRepresentation<LBSGraphData>();
        }
    }
}