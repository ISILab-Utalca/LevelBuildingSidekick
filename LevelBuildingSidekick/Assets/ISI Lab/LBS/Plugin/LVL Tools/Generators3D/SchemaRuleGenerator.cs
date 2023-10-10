using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using System.Linq;
using LBS.Bundles;
using Newtonsoft.Json;

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
    [JsonRequired]
    private float deltaWall = 1f;
    #endregion

    #region INTERNAL FIELDS
    [JsonIgnore]
    private TileMapModule tilesMod;
    [JsonIgnore]
    private ConnectedTileMapModule connectedTilesMod;
    [JsonIgnore]
    private SectorizedTileMapModule zonesMod;
    [JsonIgnore]
    private Generator3D.Settings settings;
    #endregion

    #region PPROPERTIES
    [JsonIgnore]
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;
    [JsonIgnore]
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

    public override List<Message> CheckViability(LBSLayer layer)
    {
        var msgs = new List<Message>();
        var zonesMod = layer.GetModule<SectorizedTileMapModule>();

        foreach (var zone in zonesMod.Zones)
        {
            if(zone.OutsideStyles.Count <= 0)
            {
                msgs.Add(new Message(
                    Message.Type.Warning,
                    "La zona '" + zone + "' no contiene bundles de estilo para crear el outside."
                    ));
            }

            if(zone.InsideStyles.Count <= 0)
            {
                msgs.Add(new Message(
                    Message.Type.Warning,
                    "La zona '" + zone + "' no contiene bundles de estilo para crear el inside."
                    ));
            }
        }

        return msgs;
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

    private GameObject GenerateCorners(GameObject pivot, List<Bundle> bundles, LBSTile tile)
    {
        var currents = new List<Bundle>();
        foreach (var bundle in bundles)
        {
            currents = bundle.GetChildrenByPositioning(Positioning.Corner);
        }

        var current = currents.Random();

        var selfConnections = connectedTilesMod.GetConnections(tile);
        for (int i = 0; i < Dirs.Count; i++)
        {
            var d1 = Dirs[i];

            if (!selfConnections[i].Equals("Empty")) 
                continue;

            var neigth = tilesMod.GetTileNeighbor(tile, d1);

            if (neigth == null)
                continue;

            // Get neigth connections
            var neigthConnections = connectedTilesMod.GetConnections(neigth);

            // Check if neigth connection is empty
            var j = (i + 2) % Dirs.Count;
            var k1 = (j + 1) % Dirs.Count;
            if (neigthConnections[k1] != "Empty")
            {
                // Check if self is empty
                if (!selfConnections[k1].Equals("Empty"))
                    continue;

                // Get random by weight
                var pref = current.Assets.RandomRullete(a => a.probability).obj;
                var instance = CreateObject(pref, pivot.transform);

                // Set delta position
                var dir = Dirs[i] + Dirs[k1];
                instance.transform.position = new Vector3(
                    settings.scale.x / 2f * dir.x,
                    0,
                    settings.scale.y / 2f * dir.y) * deltaWall;

                // Set rotation orientation
                var rot = (i) % Dirs.Count();
                instance.transform.rotation = Quaternion.Euler(0, -90 * rot, 0);
            }

            /*
            // Check if neigth connection is empty
            var vv = (j - 1) % Dirs.Count;
            var k2 = vv >= 0 ? vv : Dirs.Count - 1; 
            if (neigthConnections[k2] != "empty")
            {
                // Check if self is empty
                if (!selfConnections[k2].Equals("Empty"))
                    continue;

                // Get random by weight
                var pref = current.Assets.RandomRullete(a => a.probability).obj;
                var instance = CreateObject(pref, pivot.transform);

                // Set delta position
                var dir = Dirs[i] + Dirs[k2];
                instance.transform.position = new Vector3(
                    settings.scale.x / 2f * dir.x,
                    0,
                    settings.scale.y / 2f * dir.y) * deltaWall;

                // Set rotation orientation
                var rot = (i - 1) % Dirs.Count();
                instance.transform.rotation = Quaternion.Euler(0, 90 * rot, 0);
                instance.name = "R:"+ rot +" i:" + i;
            }
            */
        }

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
            var bundles = zone.GetInsideBundles();

            if(bundles.Count <= 0)
            {
                Debug.LogWarning("No se pudo finalizar la generacion de la zona '"+zone.ID+"'" +
                    "ya que no contiene bundles que definan su estilo interior");

                continue;
            }

            // Get connections
            var connections = connectedTilesMod.GetConnections(tile);

            //Generate tile
            var tileObj = new GameObject(tile.Position.ToString());

            // Add pref part to pivot
            GenerateCenters(tileObj, bundles);
            GenerateEdges(tileObj, bundles, connections);
            GenerateCorners(tileObj, bundles, tile);

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

    public override bool Equals(object obj)
    {
        var other = obj as SchemaRuleGenerator;

        if (other == null) return false;

        if (!this.deltaWall.Equals(other.deltaWall)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
