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
    private float deltaWall = 1f;
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
            currents = bundle.GetChildrenByPositioning(Positioning.Center);
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
            currents = bundle.GetChildrenByPositioning(Positioning.Edge);
        }

        for (var i = 0; i < connections.Count; i++)
        {
            // Get random bundle with respctive "connection tag"
            var current = currents.Where(b => b.GetCharacteristics<LBSTagsCharacteristic>()
                .Any(c => c.Value.name == connections[i]))
                .ToList().Random();

            // check if current is valid
            if (current == null)
            {
                Debug.Log("Los bundles no contienen elemetos con la tag: '" + connections[i] + "'");
                continue;
            }

            // Get random by weight
            var pref = current.Assets.RandomRullete(a => a.probability).obj;

            // Create part
            var obj = CreateObject(pref, pivot.transform);

            // Set rotation orientation
            if (i % 2 == 0) 
                obj.transform.rotation = Quaternion.Euler(0, (90 * (i - 1)) % 360 , 0); // if parche? (?)
            else
                obj.transform.rotation = Quaternion.Euler(0, (90 * (i - 3)) % 360, 0);

            // Set delta position
            obj.transform.position = new Vector3(
                settings.scale.x / 2f * -obj.transform.forward.x,
                0,
                settings.scale.y / 2f * -obj.transform.forward.z) * deltaWall;

        }

        return pivot;
    }

    private GameObject GenerateCorners(GameObject pivot, List<Bundle> bundles)
    {
        var currents = new List<Bundle>();
        foreach (var bundle in bundles)
        {
            currents = bundle.GetChildrenByPositioning(Positioning.Corner);
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
        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.IsPresset).ToList();
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
                new Vector3(tile.Position.x * settings.scale.x, 0, tile.Position.y * settings.scale.y) +
                - (new Vector3(settings.scale.x, 0, settings.scale.y) / 2f);

            // Set mainPivot as the parent of tileObj
            tileObj.transform.parent = mainPivot.transform;
        }

        return mainPivot;
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
