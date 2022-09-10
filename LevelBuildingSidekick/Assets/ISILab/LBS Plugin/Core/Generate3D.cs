using LBS.Representation.TileMap;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public static class Generate3D
{

    public static void GenerateLevel(LoadedLevel level)
    {
        var schema = level.data.GetRepresentation<LBSTileMapData>();
        var graph = level.data.GetRepresentation<LBSGraphData>();
        GenPhysic3D(schema, graph);
        GenPopulation3D();
    }

    public static GameObject GenPhysic3D(LBSTileMapData schema, LBSGraphData graph, string name = "New level 3D")
    {

        var mainPivot = new GameObject(name);
        foreach (var node in graph.GetNodes())
        {
            var bNames = (node as RoomCharacteristicsData).bundlesNames;
            var bundle = RoomElementBundle.Combine(bNames.Select(n => DirectoryTools.GetScriptable<RoomElementBundle>(n)).ToList());

            var room = schema.GetRoom(node.Label);
            foreach (var tile in room.Tiles)
            {
                var pivot = new GameObject();
                pivot.transform.SetParent(mainPivot.transform);
                pivot.transform.position = new Vector3(tile.x, 0, tile.y); // (* vector de tamaño de tile en mundo) (!)


                foreach (var cat in bundle.GetCategories())
                {
                    GameObject go;
                    switch (cat.pivotType)
                    {
                        case PivotType.Center:
                            go = GenPhysicCenter(cat, pivot.transform);
                            break;
                        case PivotType.Edge:
                            //go = GenPhysicEdge(tile, room, graph, cat); // implementar construccion de murrallas y puertas (!!!)
                            //go.transform.SetParent(pivot.transform);
                            break;
                    }
                }

            }
        }
        return mainPivot;
    }

    private static GameObject GenPhysicCenter(ItemCategory bundle,Transform parent)
    {
        var prefs = bundle.items;
        return SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)],parent);
    }

    /*
    private static GameObject GenPhysicEdge(Vector2Int tile,RoomData room, LBSGraphData graphData, ItemCategory cat)
    {
        if()
    }
    */

    public static void GenPopulation3D()
    {

    }
    public enum PivotType
    {
        Center,
        Edge
    }
}

