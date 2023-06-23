using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.Generator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SchemaRuleGeneratorExteriror : LBSGeneratorRule
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
        return new SchemaRuleGeneratorExteriror();
    }

    public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
    {
        schema = layer.GetModule<LBSSchema>();
        graph = layer.GetModule<LBSRoomGraph>();

        var mainPivot = new GameObject("Schema Exterior");

        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.isPreset).ToList(); // obtengo todos los bundles
        var rootBundles = allBundles.Where(b => b.IsRoot()).ToList();// obtengo todos los bundles root

        var position = settings.position;
        for (int i = 0; i < graph.NodeCount; i++) // recorro los cuartos
        {
            var node = graph.GetNode(i);
            var tags = new List<string>(node.Room.ExteriorTags);

            var currentRoots = new List<Bundle>();
            if (tags.Count > 0)
            {
                foreach (var bundle in rootBundles)
                {
                    if (bundle.ID == null)
                        continue;

                    if (tags.Contains(bundle.ID.Label))
                    {
                        currentRoots.Add(bundle);
                    }
                }
                //currentRoots = rootBundles.Where(b => tags.Contains(b.ID.Label)).ToList(); // obtengo los root de los tags correspondientes
            }
            else
            {
                currentRoots = rootBundles.ToList();
            }

            var childs = currentRoots.SelectMany(b => b.ChildsBundles).ToList().RemoveEmpties(); // obtengo todos sus hijos
            childs = childs.Where(b => b.ID != null).ToList(); // parche (?)

            var bundlesDictionary = new Dictionary<string, List<GameObject>>();

            var wallBundles = childs.Where(b => b.ID.Label.Equals("Wall")).ToList(); // obtengo todos los bundles con la tag Wall
            bundlesDictionary.Add("Wall", wallBundles.SelectMany(b => b.Assets).ToList());

            var doorBundles = childs.Where(b => b.ID.Label.Equals("Door")).ToList(); // obtengo todos los bundles con la tag Door
            bundlesDictionary.Add("Door", doorBundles.SelectMany(b => b.Assets).ToList());

            var floorBundles = childs.Where(b => b.ID.Label.Equals("Floor")); // obtengo todos los bundles con la tag Floor
            bundlesDictionary.Add("Floor", floorBundles.SelectMany(b => b.Assets).ToList());

           // var cornerBundles = childs.Where(b => b.ID.Label.Equals("Corner")).ToList(); // obtengo todos los bundles con la tag Corner
           // bundlesDictionary.Add("Corner", cornerBundles.SelectMany(b => b.Assets).ToList());

            var wallsTile = schema.GetArea(node.ID).GetWalls().SelectMany(w => w.Tiles).ToList().RemoveDuplicates();
            var tiles = wallsTile.Select(w => schema.GetTile(w) as ConnectedTile).ToList();

            for (int j = 0; j < tiles.Count; j++)
            {
                BuildTile(tiles[j], bundlesDictionary, mainPivot.transform, settings);
            }
        }
        mainPivot.transform.position = position;

        return mainPivot;
    }

    private void BuildTile(ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent, Generator3D.Settings settings)
    {
        var scale = settings.scale;
        var sideDir = new List<Vector2>() { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };
        var diagDir = new List<Vector2>()
        {
            new Vector2(1, 1),     // Diagonal superior derecha
            new Vector2(1, -1),    // Diagonal inferior derecha
            new Vector2(-1, 1),    // Diagonal superior izquierda
            new Vector2(-1, -1)    // Diagonal inferior izquierda
        };
        var pivot = new GameObject("Tile: " + tile.Position);
        pivot.transform.parent = parent;

        var sideNeigths = sideDir.Select(dir => schema.GetTileNeighbor(tile, dir.ToInt())).ToList();
        for (int i = 0; i < tile.Sides; i++)
        {
            if (sideNeigths[i] != null)
                continue;

            var tag = tile.GetConnection(i);
            if (bundles.ContainsKey(tag))
            {
                var prefabs = bundles[tag];

                if (prefabs.Count <= 0)
                {
                    Debug.LogWarning("[ISI LAB]: uno o mas bundles continen '0' Gameobject.");
                    continue;
                }
#if UNITY_EDITOR
                var wall = PrefabUtility.InstantiatePrefab(prefabs[Random.Range(0, prefabs.Count)], pivot.transform) as GameObject;
#else
                var wall =  GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
#endif
                wall.transform.position += new Vector3(sideDir[i].x * (scale.x / 2), 0, -sideDir[i].y * (scale.y / 2));
                wall.transform.rotation = Quaternion.Euler(0, (-(90 * (i + 1)) % 360) + 180, 0);
            }
        }
        
        var diagNeigths = diagDir.Select(dir => schema.GetTileNeighbor(tile, dir.ToInt())).ToList();
        for (int i = 0; i < diagDir.Count; i++)
        {
            if (diagNeigths[i] != null)
                continue;

            var tag = "Corner";
            if (bundles.ContainsKey(tag))
            {
                var prefabs = bundles[tag];

                if (prefabs.Count <= 0)
                {
                    Debug.LogWarning("[ISI LAB]: uno o mas bundles continen '0' Gameobject.");
                    continue;
                }
#if UNITY_EDITOR
                var wall = PrefabUtility.InstantiatePrefab(prefabs[Random.Range(0, prefabs.Count)], pivot.transform) as GameObject;
#else
                var wall =  GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
#endif
                wall.transform.position += new Vector3(sideDir[i].x * (scale.x / 2), 0, sideDir[i].y * (scale.y / 2));
                wall.transform.rotation = Quaternion.Euler(0, (-(90 * (i + 1)) % 360) + 180, 0);
            }
        }

        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, -scale.y * tile.Position.y) + new Vector3(scale.x, 0, -scale.y) / 2;
    }
}
