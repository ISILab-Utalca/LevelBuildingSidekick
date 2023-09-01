using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.Components.Specifics;
using System.Linq;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using LBS.Bundles;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[RequieredModule(typeof(TileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(SectorizedTileMapModule),
    typeof(ConnectedZonesModule))]
public class SchemaRuleGenerator : LBSGeneratorRule
{
    #region FIELDS
    private float deltaWall = 1;
    #endregion

    #region INTERNAL FIELDS
    private TileMapModule tilesMod;
    private ConnectedTileMapModule connectedTilesMod;
    private SectorizedTileMapModule zonesMod;
    private Generator3D.Settings settings;
    #endregion

    #region PPROPERTIES
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;
    private List<Vector2Int> DirDiags => Directions.Bidimencional.Diagonals;
    #endregion

    #region CONSTRUCTORS
    public SchemaRuleGenerator() { }
    #endregion

    #region METHODS
    public void Init(LBSLayer layer, Generator3D.Settings settings)
    {
        this.tilesMod = layer.GetModule<TileMapModule>();
        this.connectedTilesMod = layer.GetModule<ConnectedTileMapModule>();
        this.zonesMod = layer.GetModule<SectorizedTileMapModule>();
        this.settings = settings;
    }

    public override bool CheckIfIsPosible(LBSLayer layer, out string msg)
    {
        /*
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
        */
        msg = "";
        return true;
    }

    public override object Clone()
    {
        return new SchemaRuleGenerator();
    }

    private GameObject GenerateCenters(GameObject pivot ,List<Bundle> bundles)
    {
        // Get "Center" bundles 
        var currents = new List<Bundle>();
        foreach (var bundle in bundles)
        {
            currents = bundle.GetChildrensByTag("Center");
        }

        // Get random bundle
        var current = currents.Random();

        // Get random by weight
        var pref = current.Assets.RandomRullete(a => a.probability).obj;
        
        // Create part
        CreateObject(pref, pivot.transform);

        return pivot;
    }

    private GameObject GenerateEdges(GameObject pivot, List<Bundle> bundles, List<string> connections)
    {
        // Get "Edge" bundles
        var currents = new List<Bundle>();
        foreach (var bundle in bundles)
        {
            currents = bundle.GetChildrensByTag("Edge");
        }

        for (var i = 0;i < connections.Count; i++)
        {
            // Get random bundle with respctive "connection tag"
            var current = currents.Where(b => b.ID.Label == connections[i])
                                .ToList().Random();

            // Get random by weight
            var pref = current.Assets.RandomRullete(a => a.probability).obj;

            // Create part
            var obj = CreateObject(pref, pivot.transform);

            // Set rotation orientation
            obj.transform.rotation = Quaternion.Euler(0, (90 * i) % 360, 0);

            // Set delta position
            obj.transform.position = new Vector3(
                settings.scale.x * obj.transform.forward.x,
                0,
                settings.scale.y * obj.transform.forward.y) * deltaWall;

        }

        return pivot;
    }

    private GameObject GenerateCorners(GameObject pivot, List<Bundle> bundles)
    {
        var currents = new List<Bundle>();
        foreach (var bundle in bundles)
        {
            currents = bundle.GetChildrensByTag("Corner");
        }

        var current = currents.Random();
        var pref = current.Assets.RandomRullete(a => a.probability).obj;

        return pivot;
    }

    public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
    {
        // Init values
        Init(layer, settings);

        // Get bundles
        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.isPreset).ToList();
        var rootBundles = allBundles.Where(b => b.IsRoot()).ToList();

        // Create pivot
        var mainPivot = new GameObject("Schema");

        foreach (var tile in tilesMod.Tiles)
        {
            // Get zone
            var zone = zonesMod.GetZone(tile);

            // Get bundle from current tile
            var bundles = zone.GetBundles();

            // Get connections
            var connections = connectedTilesMod.GetConnections(tile);

            //Generate tile
            var tileObj = new GameObject(tile.Position.ToString());

            // Add pref part to pivot
            GenerateCenters(tileObj, bundles);
            GenerateEdges(tileObj, bundles, connections);
            GenerateCorners(tileObj, bundles);

            // Set position
            tileObj.transform.position =
                settings.position +
                new Vector3(settings.scale.x, 0, settings.scale.y) +
                new Vector3(tile.Position.x, 0, tile.Position.y) +
                (new Vector3(settings.scale.x, 0, settings.scale.y) / 2f);

            // Set mainPivot as the parent of tileObj
            tileObj.transform.parent = mainPivot.transform;

        }

        return mainPivot;

        /*
        var aaas = new List<int>() { -1, 0, 1 };
        //for (int aa = 0; aa < aaas.Count; aa++)
        {
            var ddds = new List<int>() { 1, -1 };
            //for (int dd = 0; dd < ddds.Count; dd++)
            {
                var fffs = new List<int>() { 0, 1, 2, 3 };
                //for (int ff = 0; ff < fffs.Count; ff++)
                {
                    var subMain = new GameObject("d: " + 0 + ",f: " + 1 + ",a: " + 0);

                    var position = settings.position;
                    for (int i = 0; i < graph.NodeCount; i++) // recorro los cuartos
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

                        var cornerBundles = childs.Where(b => b.ID.Label.Equals("Corner")).ToList(); // obtengo todos los bundles con la tag Corner
                        bundlesDictionary.Add("Corner", cornerBundles.SelectMany(b => b.Assets).ToList());

                        var area = schema.GetArea(node.ID);
                        for (int j = 0; j < area.TileCount; j++)
                        {
                            var tile = area.GetTile(j) as ConnectedTile;
                            BuildWall(tile, bundlesDictionary, subMain.transform, settings);
                        }

                        var corns = area.GetCorners();
                        var cc = corns.RemoveDuplicates();
                        var cornerTiles = cc.Select(w => w as ConnectedTile).ToList();
                        foreach (var tile in cornerTiles)
                        {
                            BuildCorner(area, tile, bundlesDictionary, subMain.transform, settings, fffs[1], ddds[0], aaas[0]);
                        }
                    }
                    subMain.transform.position = position;// + new Vector3(ff * 30, dd * 30, aa * 30);
                    subMain.transform.parent = mainPivot.transform;
                    //mainPivot.transform.position = position;
                }
            }
        }*/

    }

    private GameObject CreateObject(GameObject pref, Transform pivot)
    {
#if UNITY_EDITOR
        var obj = PrefabUtility.InstantiatePrefab(pref, pivot) as GameObject;
#else
        var obj =  GameObject.Instantiate(pref, pivot);
#endif
        return obj;
    }
    /*
    private void BuildCorner(TiledArea area ,ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent, Generator3D.Settings settings, int fff, int ddd, int aaa)
    {
        var scale = settings.scale;
        var sideDir = new List<Vector2>() { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };
        var diagDir = new List<Vector2>()
        {
            new Vector2(1, 1),     // Diagonal superior derecha
            new Vector2(-1, 1),    // Diagonal superior izquierda
            new Vector2(-1, -1),   // Diagonal inferior izquierda
            new Vector2(1, -1),    // Diagonal inferior derecha
        };
        var pivot = new GameObject("Tile: " + tile.Position);
        pivot.transform.parent = parent;
         

        var diagNeigths = diagDir.Select(dir => schema.GetTileNeighbor(tile, dir.ToInt())).ToList();
        for (int i = 0; i < diagDir.Count; i++)
        {
            if (diagNeigths[i] != null)// || area.Contains(diagNeigths[i].Position))
                continue;

            var comp = diagDir[i].AsComponents();
            var others = comp.Select(dir => area.GetTile(tile.Position + dir.ToInt())).ToList();
            //var others = comp.Select(dir => schema.GetTileNeighbor(tile, dir.ToInt())).ToList();
            if (!others.All(t => t != null))// || !area.Contains(t.Position)))
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

                var corner = CreateObject(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                corner = FixCornerOrientation(corner,  diagDir[i], scale, i, ddd, fff * 90, aaa);
            }
        }

        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, -scale.y * tile.Position.y) + new Vector3(scale.x, 0, -scale.y) / 2;
    }


    private void BuildWall(ConnectedTile tile, Dictionary<string, List<GameObject>> bundles, Transform parent, Generator3D.Settings settings)
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

        var floor = CreateObject(bases[Random.Range(0, bases.Count)], pivot.transform);

        //for (int i = 0; i < tile.Sides; i++)
        for (int i = 0; i < 4; i++)
        {
            var tag = tile.GetConnection(i);
            if (bundles.ContainsKey(tag))
            {
                var prefabs = bundles[tag];

                if (prefabs.Count <= 0)
                {
                    Debug.LogWarning("[ISI LAB]: uno o mas bundles continen '0' Gameobject.");
                    return;
                }

                var wall = CreateObject(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                wall = FixOrientation(wall, sideDir[i], scale, i);

                //var wall =  SceneView.Instantiate(prefabs[Random.Range(0, prefabs.Count)], pivot.transform);
                //wall.transform.position += new Vector3(sideDir[i].x*(scale.x/2), 0, sideDir[i].y*(scale.y/2));
                //wall.transform.rotation = Quaternion.Euler(0, -(90 * (i + 1)) % 360, 0);
            }
        }
        pivot.transform.position = new Vector3(scale.x * tile.Position.x, 0, -scale.y * tile.Position.y) + new Vector3(scale.x, 0, -scale.y) / 2;
    }
    


    private GameObject FixOrientation(GameObject obj, Vector2 dir, Vector2 scale, int rotation)
    {
        obj.transform.position += new Vector3(dir.x * (scale.x / 2), 0, dir.y * (scale.y / 2));
        obj.transform.rotation = Quaternion.Euler(0, (-(90 * (rotation + 1)) % 360), 0); // -90 * (r + 1) los numeros son parche de la direnecia de orden de las direcciones (!)
        return obj;
    }

    private GameObject FixCornerOrientation(GameObject obj, Vector2 dir, Vector2 scale, int rotation, int ddd, int fff, int aaa)
    {
        Debug.Log(obj.transform.parent.name + ", " + dir + ", " + rotation);
        obj.transform.position += new Vector3(dir.x * (scale.x / 2), 0, -dir.y * (scale.y / 2));
        obj.transform.rotation = Quaternion.Euler(0, (ddd * (90 * (rotation + aaa)) % 360) + fff, 0); // -90 * (r + 1) los numeros son parche de la direnecia de orden de las direcciones (!)
        return obj;
    }*/
    #endregion
}
