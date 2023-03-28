using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.Components.Specifics;
using System.Linq;
//using UnityEditor;

[System.Serializable]
public class SchemaGenerator : Generator3D
{
    LBSSchema schema;
    LBSRoomGraph graph;

    public override GameObject Generate(LBSLayer layer)
    {
        Init(layer);

        var mainPivot = new GameObject(objName);

        for(int i = 0; i < graph.NodeCount; i++)
        {
            var node = graph.GetNode(i);
            var tags = node.Room.Tags;

            var bundlesDictionary = new Dictionary<string, List<GameObject>>();

            var bundlesSO = Utility.DirectoryTools.GetScriptables<CompositeBundle>();
            var temp = tags.Select(s => s.Clone() as string).ToList();

            bundlesDictionary.Add("Wall", bundlesSO.SelectMany(b => b.GetObjects("Wall", temp)).ToList());
            bundlesDictionary.Add("Door", bundlesSO.SelectMany(b => b.GetObjects("Door", temp)).ToList());
            bundlesDictionary.Add("Floor", bundlesSO.SelectMany(b => b.GetObjects("Floor", temp)).ToList());

            var area = schema.GetArea(node.ID);
            for (int j = 0; j < area.TileCount; j++)
            {
                var tile = area.GetTile(j) as ConnectedTile;
                BuildTile(tile, bundlesDictionary, mainPivot.transform);
            }
        }

        mainPivot.transform.position = position;
        //mainPivot.transform.localScale = new Vector3(resize.x, 1, resize.y);

        return mainPivot;
    }

    public void BuildTile(ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent)
    {
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        var pivot = new GameObject("Tile: " + tile.Position);
        pivot.transform.parent = parent;

        var bases = bundles["Floor"];

        if(bases.Count <= 0)
        {
            Debug.LogWarning("[ISI LAB]: uno o mas bundles continen '0' Gameobject.");
            return;
        }

        var index = Random.Range(0, bases.Count);
        var floor = GameObject.Instantiate(bases[index], pivot.transform);
        //var floor = SceneView.Instantiate(bases[Random.Range(0, bases.Count)], pivot.transform);

        for (int k = 0; k < tile.Sides; k++)
        {
            var tag = tile.GetConnection(k);
            if (bundles.ContainsKey(tag))
            {
                var prefabs = bundles[tag];

                if (prefabs.Count <= 0)
                {
                    Debug.LogWarning("[ISI LAB]: uno o mas bundles continen '0' Gameobject.");
                    return;
                }

                var wall =  GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                //var wall =  SceneView.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                wall.transform.position += new Vector3(sideDir[k].x*(scale.x/2), 0, sideDir[k].y*(scale.y/2));
                wall.transform.rotation = Quaternion.Euler(0, -(90 * (k + 1)) % 360, 0);
            }
        }
        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, -scale.y * tile.Position.y);
    }

    public override void Init(LBSLayer layer)
    {
        schema = layer.GetModule<LBSSchema>();
        graph = layer.GetModule<LBSRoomGraph>();
    }
}
