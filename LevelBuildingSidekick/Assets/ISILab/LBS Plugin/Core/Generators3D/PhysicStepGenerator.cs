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
                                for (int i = 0; i < 4; i++) // esto podria cambiar si hay tiles hexagonales o triangulares (?)
                                {
                                    go = GenPhysicEdge(tile, room, graph, cat); // implementar construccion de murrallas y puertas (!!!)
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

        private GameObject GenPhysicEdge(TileData tile, RoomData room, LBSGraphData graphData, ItemCategory cat)
        {
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