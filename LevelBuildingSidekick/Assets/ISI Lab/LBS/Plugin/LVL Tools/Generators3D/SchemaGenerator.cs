using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.Components.Specifics;
using System.Linq;
using UnityEditor;

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

            // Debería sacarse del Schema, guardarse en un diccionario strig List<GameObject> y ser un ciclo
            //Debería estar en una clase de ensamblado de tiles
            var bundles = new Dictionary<string, List<GameObject>>();

            var temp = new List<string>(tags);
            temp.Add("Wall"); 
            bundles.Add("Wall", layer.Bundle.GetObjects(temp));

            temp = new List<string>(tags);
            temp.Add("Door");
            bundles.Add("Door", layer.Bundle.GetObjects(temp));

            temp = new List<string>(tags);
            temp.Add("Center");
            bundles.Add("Center", layer.Bundle.GetObjects(temp));

            var area = schema.GetArea(node.ID);

            for(int j = 0; j < area.TileCount; j++)
            {
                var tile = area.GetTile(i) as ConnectedTile;

                BuildTile(tile, bundles, mainPivot.transform);
            }
        }

        mainPivot.transform.position = position;

        return mainPivot;

    }

    public void BuildTile(ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent)
    {
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        var pivot = new GameObject("Tile: " + tile.Position);

        var bases = bundles["Base"];
        var floor = SceneView.Instantiate(bases[Random.Range(0, bases.Count)], parent);

        for (int k = 0; k < tile.Sides; k++)
        {
            var tag = tile.GetConnection(k);
            if (bundles.ContainsKey(tag))
            {
                var prefabs = bundles[tag];

                var wall = SceneView.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                var delta = sideDir[k] / 2;
                wall.transform.position += new Vector3(delta.x * scale.x, 0, delta.y * scale.y);
                wall.transform.rotation = Quaternion.Euler(0, (90 * (k + 1)) % 360, 0);
            }
        }

        pivot.transform.localScale = new Vector3(scale.x, 1, scale.y);
        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, scale.y * tile.Position.y);
    }

    public override void Init(LBSLayer layer)
    {
        schema = layer.GetModule<LBSSchema>();
        graph = layer.GetModule<LBSRoomGraph>();
    }
}
