using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.Components.Specifics;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SchemaRuleGenerator : LBSGeneratorRule
{
    private LBSSchema schema;
    private LBSRoomGraph graph;

    public override bool CheckIfIsPosible(LBSLayer layer, out string msg)
    {
        msg = "";

        var schema = layer.GetModule<LBSSchema>();
        var graph = layer.GetModule<LBSRoomGraph>();
        if (schema == null)
        {
            msg = "The layer does not contain any module corresponding to 'LBSSchema'.";
            return false;
        }
        if (graph == null)
        {
            msg = "The layer does not contain any module corresponding to 'LBSRoomGraph'.";
            return false;
        }
        return true;
    }

    public override object Clone()
    {
        return new SchemaRuleGenerator();
    }

    public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
    {   
        schema = layer.GetModule<LBSSchema>();
        graph = layer.GetModule<LBSRoomGraph>();

        var mainPivot = new GameObject("Schema");

        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.isPreset).ToList(); // obtengo todos los bundles
        var rootBundles = allBundles.Where(b => b.IsRoot()).ToList();// obtengo todos los bundles root

        var position = settings.position;
        for(int i = 0; i < graph.NodeCount; i++) // recorro los cuartos
        {
            var node = graph.GetNode(i);
            var tags = new List<string>(node.Room.InteriorTags);

            var currentRoots = new List<Bundle>();
            if (tags.Count > 0)
            {
                foreach (var bundle in rootBundles)
                {
                    if (bundle.ID == null)
                        continue;

                    if(tags.Contains(bundle.ID.Label))
                    {
                        currentRoots.Add(bundle);
                    }
                }
                //currentRoots = rootBundles.Where(b => tags.Contains(b.ID.Label)).ToList(); // obtengo los root de los tags correspondientes
            }
            else
                currentRoots = rootBundles.ToList();

            var childs = currentRoots.SelectMany(b => b.ChildsBundles).ToList().RemoveEmpties(); // obtengo todos sus hijos
            childs = childs.Where(b => b.ID != null).ToList(); // parche (?)

            var bundlesDictionary = new Dictionary<string, List<GameObject>>();

            var wallBundles = childs.Where(b => b.ID.Label.Equals("Wall")).ToList(); // obtengo todos los bundles con la tag Wall
            bundlesDictionary.Add("Wall", wallBundles.SelectMany(b => b.Assets).ToList());

            var doorBundles = childs.Where(b => b.ID.Label.Equals("Door")).ToList(); // obtengo todos los bundles con la tag Door
            bundlesDictionary.Add("Door", doorBundles.SelectMany(b => b.Assets).ToList());

            var floorBundles = childs.Where(b => b.ID.Label.Equals("Floor")); // obtengo todos los bundles con la tag Floor
            bundlesDictionary.Add("Floor", floorBundles.SelectMany(b => b.Assets).ToList());

            var cornerBundles = childs.Where(b => b.ID.Label.Equals("Corner")).ToList(); // obtengo todos los bundles con la tag Corner
            bundlesDictionary.Add("Corner", cornerBundles.SelectMany(b => b.Assets).ToList());

            var area = schema.GetArea(node.ID);
            for (int j = 0; j < area.TileCount; j++)
            {
                var tile = area.GetTile(j) as ConnectedTile;
                BuildTile(tile, bundlesDictionary, mainPivot.transform,settings);
            }
        }

        mainPivot.transform.position = position;

        return mainPivot;
    }

    private void BuildTile(ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent, Generator3D.Settings settings)
    {
        var scale = settings.scale;
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

#if UNITY_EDITOR
        var floor = PrefabUtility.InstantiatePrefab(bases[index], pivot.transform);
#else
        var floor = GameObject.Instantiate(bases[index], pivot.transform);
#endif
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
#if UNITY_EDITOR
                var wall = PrefabUtility.InstantiatePrefab(prefabs[Random.Range(0, prefabs.Count)], pivot.transform) as GameObject;
#else
                var wall =  GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
#endif
                //var wall =  SceneView.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                wall.transform.position += new Vector3(sideDir[k].x*(scale.x/2), 0, sideDir[k].y*(scale.y/2));
                wall.transform.rotation = Quaternion.Euler(0, -(90 * (k + 1)) % 360, 0);
            }
        }
        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, -scale.y * tile.Position.y) + new Vector3(scale.x, 0, -scale.y) / 2;
    }

}
