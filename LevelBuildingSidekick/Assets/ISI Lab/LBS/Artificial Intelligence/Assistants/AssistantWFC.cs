using System.Collections.Generic;
using System.Linq;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule), typeof(ConnectedTileMapModule))]
    public class AssistantWFC : LBSAssistant
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private bool overrideValues;

        [SerializeField, InspectorName("Target Bundle")]
        private Bundle targetBundleRef;
        
        // Stores the guid for the object instead of the current path
        [FormerlySerializedAs("targetBundle")] [SerializeField, JsonRequired, HideInInspector]
        private string bundlePath = "";

        /***
         * Use asset's GUID; current bundle:
         * - "Exterior_Plains" 
         */
        private string defaultBundleGuid = "9d3dac0f9a486fd47866f815b4fefc29";

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public bool OverrideValues
        {
            get => overrideValues;
            set => overrideValues = value;
        }

        [JsonIgnore]
        public List<Vector2Int> Positions { get; set; }

        [JsonIgnore]
        public Bundle TargetBundle
        {
            get => GetBundle(bundlePath);
            set
            {
                targetBundleRef = value;
                OnGUI();
            }
        }
        
        public Bundle Bundle
        {
            get
            {
                if(bundlePath != "") 
                {
                    var bundle = AssetDatabase.LoadAssetAtPath<Bundle>(bundlePath); // The custom bundle
                    Debug.Log(bundle);
                    return bundle;
                }
          
                var guiBundle = AssetDatabase.LoadAssetAtPath<Bundle>(AssetDatabase.GUIDToAssetPath(defaultBundleGuid)); // the default bundle
                Debug.Log(guiBundle);
                return guiBundle;

            }
            set => targetBundleRef = value;
        }

        private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;
        #endregion

        #region CONSTRUCTORS

        public AssistantWFC(Texture2D icon, string name) : base(icon, name)
        {
            OnGUI(); 
        }
        
        #endregion

        #region METHODS

        public sealed override void OnGUI()
        {
            if (!targetBundleRef)
            {
                bundlePath = AssetDatabase.GUIDToAssetPath(defaultBundleGuid);
            }
            else
            {
                bundlePath = AssetDatabase.GetAssetPath(targetBundleRef);
                defaultBundleGuid = AssetDatabase.AssetPathToGUID(bundlePath);
            }
            targetBundleRef = AssetDatabase.LoadAssetAtPath<Bundle>(bundlePath);
        }

        public override object Clone()
        {
            return new AssistantWFC(this.Icon, this.Name);
        }

        public void Execute()
        {
            // Get Bundle
            OnGUI();
            var bundle = targetBundleRef;// GetBundle(targetBundle);

            // Cheack if can execute
            if (bundle == null)
            {
                Debug.LogWarning("No bundle is selected.");
                return;
            }

            // Get bundles posible tiles
            var group = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];

            // Get modules
            var map = Owner.GetModule<TileMapModule>();
            var connected = Owner.GetModule<ConnectedTileMapModule>();

            // Get tiles to change
            var toCalc = GetTileToCalc(Positions, map, connected);

            // Create auxiliar collections
            var closed = new List<LBSTile>();
            var reCalc = new List<LBSTile>();

            //Init
            var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();
            foreach (var tile in toCalc)
            {
                // Get candidates related to current tile
                var candidates = CalcCandidates(tile, group);
                currentCalcs.Add(tile, candidates);
            }

            // Run as long as you have tiles 
            while (toCalc.Count > 0)
            {
                var _closed = new List<LBSTile>(closed);

                // end condition
                var xx = currentCalcs.Where(e => e.Value.Count > 1).ToList();
                if (xx.Count <= 0)
                    break;

                // Get tile with lees possibilities
                var current = xx.OrderBy(e => e.Value.Count).First();

                // cheack if curren tile have tile posibilities
                if (current.Value.Count <= 0)
                {
                    // Remove from the list of tiles to calculate 
                    Debug.Log(current.Key.Position + " no tiene posibles tile.");
                    toCalc.Remove(current.Key);
                    continue;
                }

                // Collapse posibilities
                var selected = current.Value.RandomRullete(c => c.weigth);
                var connections = selected.bundle.GetConnection(selected.rotation);
                connected.SetConnections(current.Key, connections.ToList(), new List<bool>() { false, false, false, false });
                currentCalcs[current.Key] = new List<Candidate>() { selected };

                // Ignore This tiles
                closed.Add(current.Key);

                // Collapse neigthbours connection 
                var neigth = map.GetTileNeighbors(current.Key, Dirs);
                SetConnectionNei(current.Key, neigth.ToArray(), closed);

                // Add to reCalc list
                var neigthCalcs = neigth.RemoveEmpties().Where(n => currentCalcs.Any(c => c.Key == n)).ToList();
                reCalc.AddRange(neigthCalcs);

                while (reCalc.Count > 0)
                {
                    var tile = reCalc.First();

                    // Get candidates related to current tile
                    List<Candidate> lastCandidates;
                    currentCalcs.TryGetValue(tile, out lastCandidates);
                    var newCandidates = CalcCandidates(tile, group);

                    if (lastCandidates == null || newCandidates.Count < lastCandidates.Count)
                    {
                        currentCalcs[tile] = newCandidates;

                        // Get neigthbours
                        var neigs = map.GetTileNeighbors(tile, Dirs).RemoveEmpties();

                        // Add to reCalc list
                        foreach (var nei in neigs)
                        {
                            // Check if tile is closed
                            if (_closed.Contains(nei))
                                continue;

                            if (reCalc.Contains(nei))
                                continue;

                            reCalc.Add(nei);
                        }
                    }
                    reCalc.Remove(tile);
                    _closed.Add(tile);
                }

                // Remove from the list of tiles to calculate 
                toCalc.Remove(current.Key);
            }
        }

        private List<LBSTile> GetTileToCalc(List<Vector2Int> positions, TileMapModule map, ConnectedTileMapModule connected)
        {
            var toR = new List<LBSTile>();
            foreach (var position in Positions)
            {
                // Get tile inforamtion
                var tile = map.GetTile(position);

                // Check if tile is null
                if (tile == null)
                    continue;

                // Get connections
                var connection = connected.GetConnections(tile);

                if (overrideValues)
                {
                    //Clear prev conection
                    connected.SetConnections(tile,
                        new List<string>() { "", "", "", "" },
                        new List<bool>() { false, false, false, false });
                }

                toR.Add(tile);
            }
            return toR;
        }

        private List<Candidate> CalcCandidates(LBSTile tile, LBSDirectionedGroup group)
        {
            // Get modules
            var connectedMod = Owner.GetModule<ConnectedTileMapModule>();

            var candidates = new List<Candidate>();
            for (int i = 0; i < group.Weights.Count; i++)
            {
                // Get characteristics and weigth
                var weigth = group.Weights[i].weight;
                var sBundle = group.Weights[i].target.GetCharacteristics<LBSDirection>()[0];

                for (int j = 0; j < 4; j++)
                {
                    // Get connection rotated
                    var array = sBundle.GetConnection(j); //(!)

                    // Check if is valid rotated connection
                    var connections = connectedMod.GetConnections(tile);
                    if (Compare(connections.ToArray(), array))
                    {
                        var candidate = new Candidate()
                        {
                            bundle = sBundle,
                            weigth = weigth,
                            rotation = j,
                        };

                        candidates.Add(candidate);
                    }
                }
            }

            return candidates;
        }

        public void SetConnectionNei(LBSTile origin, LBSTile[] neis, List<LBSTile> closed)
        {
            var connected = Owner.GetModule<ConnectedTileMapModule>();

            var dirs = Directions.Bidimencional.Edges;

            var oring = connected.GetConnections(origin);

            for (int i = 0; i < neis.Length; i++)
            {
                if (neis[i] == null)
                    continue;

                if (closed.Contains(neis[i]))
                    continue;

                var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));

                connected.SetConnection(neis[i], idir, oring[i], false);
            }
        }

        public bool Compare(string[] a, string[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    if (a[i] != b[i] && !string.IsNullOrEmpty(a[i]) && !string.IsNullOrEmpty(a[i]))
                        return false;
                }
            }
            return true;
        }

        public Bundle GetBundle(string bundleID)
        {
            // Get Target bundle
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            foreach (var bundle in bundles)
            {
                if (bundle.name == bundleID)
                {
                    return bundle;
                }
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AssistantWFC;

            if (other == null) return false;

            if (!other.Name.Equals(this.Name)) return false;

            if (!other.bundlePath.Equals(this.bundlePath)) return false;

            if (!other.overrideValues.Equals(this.overrideValues)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    public class Candidate
    {
        public float weigth;
        public LBSDirection bundle;
        public int rotation;
    }
}