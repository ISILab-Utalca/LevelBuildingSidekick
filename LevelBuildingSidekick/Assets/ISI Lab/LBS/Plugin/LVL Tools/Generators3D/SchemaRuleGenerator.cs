using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Components.TileMap;
using System.Linq;
using LBS.Bundles;
using Newtonsoft.Json;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.LBS.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ISILab.LBS.Generators
{
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
        public override object Clone()
        {
            return new SchemaRuleGenerator();
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
                if (zone.OutsideStyles.Count <= 0)
                {
                    msgs.Add(new Message(
                        Message.Type.Warning,
                        "La zona '" + zone + "' no contiene bundles de estilo para crear el outside."
                        ));
                }

                if (zone.InsideStyles.Count <= 0)
                {
                    msgs.Add(new Message(
                        Message.Type.Warning,
                        "La zona '" + zone + "' no contiene bundles de estilo para crear el inside."
                        ));
                }
            }

            return msgs;
        }

        /// <summary>
        /// Geenerate elements correspoding to center in the bundle provided
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="bundles"></param>
        /// <returns></returns>
        private GameObject GenerateCenters(GameObject pivot, List<Bundle> bundles)
        {
            // Get "Center" bundles 
            var currents = new List<Bundle>();
            foreach (var bundle in bundles)
            {
                currents = bundle.GetChildrenByPositioning(Positioning.Center);
            }

            var tChar = currents.SelectMany(b => b.GetCharacteristics<LBSTagsCharacteristic>()).ToList();
            var tags = tChar.Select(s => s.Value.Label).ToList();
            tags.RemoveDuplicates();

            for (int i = 0; i < tags.Count; i++)
            {
                var xx = currents.Where(b => b.GetCharacteristics<LBSTagsCharacteristic>().Any(c => c.Value.name == tags[i])).ToList();

                // Get random bundle
                var current = xx.Random();

                // Get random by weight
                var pref = current.Assets.RandomRullete(a => a.probability).obj;

                // Create part
                CreateObject(pref, pivot.transform);
            }

            return pivot;
        }

        /// <summary>
        /// Generate edges of the tile based on the connections of the tile with its neighbors
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="bundles"></param>
        /// <param name="connections"></param>
        /// <returns></returns>
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
                    continue;
                }

                // Get random by weight
                var pref = current.Assets.RandomRullete(a => a.probability).obj;

                // Create part
                var obj = CreateObject(pref, pivot.transform);

                // Set rotation orientation
                if (i % 2 == 0)
                    obj.transform.rotation = Quaternion.Euler(0, (90 * (i - 1)) % 360, 0);
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

        /// <summary>
        /// Generate corner based on bundles provided
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="bundles"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
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
                var d2 = Dirs[(i + 1) % Dirs.Count];

                // if directions are NOT empty continue
                if (!selfConnections[i].Equals("Empty") || !selfConnections[(i + 1)% Dirs.Count].Equals("Empty"))
                    continue;

                var neigth = tilesMod.GetTileNeighbor(tile, d1);
                var neigth2 = tilesMod.GetTileNeighbor(tile, d2);

                // if neigths are null continue
                if (neigth == null || neigth2 == null)
                    continue;

                // Get neigth connections
                var neigthConnections = connectedTilesMod.GetConnections(neigth);
                var neigthConnections2 = connectedTilesMod.GetConnections(neigth2);

                if (neigthConnections[(i + 1) % Dirs.Count] != "Empty" || neigthConnections2[i] != "Empty")
                {
                    // Get random by weight
                    var pref = current.Assets.RandomRullete(a => a.probability).obj;
                    var instance = CreateObject(pref, pivot.transform);

                    // Set delta position
                    var dir = Dirs[i] + Dirs[(i + 1) % Dirs.Count];
                    instance.transform.position = new Vector3(
                        settings.scale.x / 2f * dir.x,
                        0,
                        settings.scale.y / 2f * dir.y) * deltaWall;

                    // Set rotation orientation
                    var rot = (i) % Dirs.Count();
                    instance.transform.rotation = Quaternion.Euler(0, -90 * (rot + 1), 0);
                }

            }

            return pivot;
        }

        /// <summary>
        /// Generate in 3D the schema of the layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Init values
            Init(layer, settings);

            // Get bundles
            var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().ToList();
            var rootBundles = allBundles.Where(b => b.IsRoot()).ToList();

            // Create pivot
            var mainPivot = new GameObject("Schema");

            var tiles = new List<GameObject>();

            foreach (var tile in tilesMod.Tiles)
            {
                // Get zone
                var zone = zonesMod.GetZone(tile);

                // Get bundle from current tile
                var bundles = zone.GetInsideBundles();

                if (bundles.Count <= 0)
                {
                    Debug.LogWarning("[ISI Lab]: Could not finish generating zone '" + zone.ID + "'" +
                    " since it does not contain bundles defining its interior style");

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

                var basePos = settings.position;
                var tilePos = new Vector3(tile.Position.x * settings.scale.x, 0, tile.Position.y * settings.scale.y);
                var delta = new Vector3(settings.scale.x, 0, settings.scale.y) / 2f;
                // Set General position
                tileObj.transform.position = basePos + tilePos - delta;


                // Set mainPivot as the parent of tileObj
                tiles.Add(tileObj);
            }

            var x = tiles.Average(t => t.transform.position.x);
            var y = tiles.Min(t => t.transform.position.y);
            var z = tiles.Average(t => t.transform.position.z);

            mainPivot.transform.position = new Vector3(x,y,z);

            foreach ( var tile in tiles ) 
            {
                tile.transform.parent = mainPivot.transform;
            }

            mainPivot.transform.position += settings.position;

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
        #endregion
    }
}