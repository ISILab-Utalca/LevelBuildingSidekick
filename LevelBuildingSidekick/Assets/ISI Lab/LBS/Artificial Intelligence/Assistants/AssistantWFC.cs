using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule), typeof(ConnectedTileMapModule))]
    public class AssistantWFC : LBSAssistant
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private bool overrideValues;

        [JsonProperty, SerializeReference, SerializeField, JsonRequired]
        private Bundle targetBundleRef;
        
        /***
         * Use asset's GUID; current bundle:
         * - "Exterior_Plains" 
         */
        private string defaultBundleGuid = "9d3dac0f9a486fd47866f815b4fefc29";

        private string presetsSaveFolder;

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
        public Bundle Bundle
        {
            get => GetBundleRef();
            set => targetBundleRef = value;
        }
        

        private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;

        #endregion

        #region CONSTRUCTORS

        public AssistantWFC(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
            OnGUI(); 
        }
        
        #endregion

        #region METHODS

        public sealed override void OnGUI()
        {
            GetBundleRef();
        }

        public override object Clone()
        {
            return new AssistantWFC(this.Icon, this.Name, this.ColorTint);
        }
        /// <summary>
        /// This new version, is similar but it constraints where the wave function collapse is applied, to the selected tiles only
        /// </summary>
        public void Execute()
        {
            // Get Bundle
            OnGUI();
            var bundle = targetBundleRef;

            if (bundle == null)
            {
                Debug.LogWarning("No bundle selected.");
                return;
            }

            var group = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];
            var map = OwnerLayer.GetModule<TileMapModule>();
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

            // Get tiles to change
            var toCalc = GetTileToCalc(Positions, map, connected);

            // Build whitelist (positions + direct neighbors)
            var whitelist = new HashSet<Vector2Int>();
            foreach (var tile in toCalc)
            {
                whitelist.Add(tile.Position);
                var neighbors = map.GetTileNeighbors(tile, Dirs);
                foreach (var n in neighbors.RemoveEmpties())
                {
                    whitelist.Add(n.Position);
                }
            }

            var closed = new List<LBSTile>();
            var reCalc = new List<LBSTile>();
            var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();

            foreach (var tile in toCalc)
            {
                var candidates = CalcCandidates(tile, group);
                currentCalcs.Add(tile, candidates);
            }

            while (toCalc.Count > 0)
            {
                var _closed = new List<LBSTile>(closed);
                var xx = currentCalcs.Where(e => e.Value.Count > 1).ToList(); //KVP de tiles y lista de candidatos (mayores a 1)
                if (xx.Count <= 0)
                    break;

                var current = xx.OrderBy(e => e.Value.Count).First();

                if (current.Value.Count <= 0)
                {
                    Debug.Log(current.Key.Position + " no tiene posibles tile.");
                    toCalc.Remove(current.Key);
                    continue;
                }

                var selected = current.Value.RandomRullete(c => c.weigth); //Elige el vecino aleatorio segun peso
                var connections = selected.bundle.GetConnection(selected.rotation);
                connected.SetConnections(current.Key, connections.ToList(), new List<bool>() { false, false, false, false });
                currentCalcs[current.Key] = new List<Candidate>() { selected };
                closed.Add(current.Key);

                var neigth = map.GetTileNeighbors(current.Key, Dirs);
                SetConnectionNei(current.Key, neigth.ToArray(), closed, whitelist);

                var neigthCalcs = neigth.RemoveEmpties()
                                         .Where(n => currentCalcs.ContainsKey(n) && whitelist.Contains(n.Position))
                                         .ToList();
                reCalc.AddRange(neigthCalcs);

                while (reCalc.Count > 0)
                {
                    var tile = reCalc.First();

                    if (!whitelist.Contains(tile.Position))
                    {
                        reCalc.Remove(tile);
                        continue;
                    }

                    currentCalcs.TryGetValue(tile, out var lastCandidates);
                    var newCandidates = CalcCandidates(tile, group);

                    if (lastCandidates == null || newCandidates.Count < lastCandidates.Count)
                    {
                        currentCalcs[tile] = newCandidates;

                        var neigs = map.GetTileNeighbors(tile, Dirs).RemoveEmpties();
                        foreach (var nei in neigs)
                        {
                            if (_closed.Contains(nei) || reCalc.Contains(nei))
                                continue;

                            if (whitelist.Contains(nei.Position))
                                reCalc.Add(nei);
                        }
                    }

                    reCalc.Remove(tile);
                    _closed.Add(tile);
                }

                toCalc.Remove(current.Key);
            }
        }

        public void SetConnectionNei(LBSTile origin, LBSTile[] neis, List<LBSTile> closed, HashSet<Vector2Int> whitelist)
        {
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();
            var dirs = Directions.Bidimencional.Edges;
            var oring = connected.GetConnections(origin);

            for (int i = 0; i < neis.Length; i++)
            {
                var nei = neis[i];
                if (nei == null || closed.Contains(nei))
                    continue;

                if (!whitelist.Contains(nei.Position))
                    continue;

                var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));
                connected.SetConnection(nei, idir, oring[i], false);
            }
        }

        public void OLDExecute()
        {
            // Get Bundle
            OnGUI();
            var bundle = targetBundleRef;// GetBundle(targetBundle);

            // Cheack if can execute
            if (bundle == null)
            {
                Debug.LogWarning("No bundle selected.");
                return;
            }

            // Get bundles posible tiles
            var group = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];

            // Get modules
            var map = OwnerLayer.GetModule<TileMapModule>();
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

            // Get tiles to change
            var toCalc = GetTileToCalc(Positions, map, connected);

            // Create auxiliar collections
            var closed = new List<LBSTile>();
            var reCalc = new List<LBSTile>();

            //Init
            var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();
            foreach (var tile in toCalc)
            {
                Debug.Log("tile:" + tile.Position);
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

                // cheack if curren tile have tile possibilities
                if (current.Value.Count <= 0)
                {
                    // Remove from the list of tiles to calculate 
                    Debug.Log(current.Key.Position + " no tiene posibles tile.");
                    toCalc.Remove(current.Key);
                    continue;
                }

                // Collapse possibilities
                var selected = current.Value.RandomRullete(c => c.weigth);
                var connections = selected.bundle.GetConnection(selected.rotation);
                connected.SetConnections(current.Key, connections.ToList(), new List<bool>() { false, false, false, false });
                currentCalcs[current.Key] = new List<Candidate>() { selected };

                // Ignore This tiles
                closed.Add(current.Key);

                // Collapse neighbours connection 
                var neigth = map.GetTileNeighbors(current.Key, Dirs);
                OLDSetConnectionNei(current.Key, neigth.ToArray(), closed);

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

                        // Get neighbours
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
                // Get tile information
                var tile = map.GetTile(position);

                // Check if tile is null
                if (tile == null)
                    continue;

                // Get connections
                var connection = connected.GetConnections(tile);

                if (overrideValues)
                {
                    //Clear prev connection
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
            var connectedMod = OwnerLayer.GetModule<ConnectedTileMapModule>();

            var candidates = new List<Candidate>();
            for (int i = 0; i < group.Weights.Count; i++)
            {
                // Get characteristics and weigh
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

        public void OLDSetConnectionNei(LBSTile origin, LBSTile[] neis, List<LBSTile> closed)
        {
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

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

        public void CopyWeights()
        {
            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

            var currentBundles = new List<Bundle>();
            group.Weights.ForEach(ws => currentBundles.Add(ws.target));

            var bundleFrequency = new Dictionary<Bundle, int>();
            int maxFreq = 0;
            currentBundles.ForEach(b => bundleFrequency.Add(b, 0));

            for(int i = 0; i < connected.Pairs.Count; i++)
            {
                bool matchFound = false;
                var tileConns = connected.Pairs[i].Connections;
                for(int j = 0; j < currentBundles.Count; j++)
                {
                    Bundle bundle = currentBundles[j];
                    var bundleConns = bundle.GetCharacteristics<LBSDirection>()[0].Connections;
                    for (int k = 0; k < 4; k++)
                    {
                        var rotatedBundleConns = bundleConns.Rotate(k);

                        if(Compare(tileConns.ToArray(), rotatedBundleConns.ToArray()))
                        {
                            bundleFrequency[bundle]++;
                            if(bundleFrequency[bundle] > maxFreq)
                                maxFreq = bundleFrequency[bundle];
                            matchFound = true;
                            j = currentBundles.Count;
                            break;
                        }
                    }
                }

                if (!matchFound)
                    Debug.LogWarning($"Tile {connected.Pairs[i].Tile.Position} has no matching bundle");
            }
            
            for (int i = 0; i < currentBundles.Count; i++) 
            {
                //Debug.Log($"{currentBundles[i]} Frequency: {bundleFrequency[currentBundles[i]]}");
                group.Weights[i].weight = maxFreq != 0 ? (float)bundleFrequency[currentBundles[i]] / (float)maxFreq : 1;
            }

            Selection.activeObject = targetBundleRef;
        }

        public void SaveWeights(string presetName, string folder, out string endName)
        {
            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];
            WFCPreset newPreset = ScriptableObject.CreateInstance<WFCPreset>();
            endName = presetName;
            if (endName.Length == 0)
            {
                endName = "New WFC Preset";
            }
            if(endName == "New WFC Preset")
            {
                int count = AssetDatabase.FindAssets(endName).Length;
                if(count > 0)
                {
                    endName += $" ({count})";
                }
            }
            newPreset.name = endName;
            newPreset.SetWeights(group.Weights);

            AssetDatabase.CreateAsset(newPreset, folder + "/" + endName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = newPreset;
        }

        public void LoadWeights(WFCPreset preset)
        {
            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];
            // Hacer match por bundle?
            //foreach(var ws in group.Weights) 
            for (int i = 0; i < group.Weights.Count; i++)
            {
                bool found = false;
                foreach (var presetWS in preset.GetWeights()) 
                {
                    if (group.Weights[i].target.Equals(presetWS.target))
                    {
                        group.Weights[i].weight = presetWS.weight;
                        found = true;
                        break;
                    }
                }
                // Testear cambiando los bundles hijos
                if(!found)
                    Debug.LogWarning($"Bundle {group.Weights[i].target} was not in preset {preset.name}");
            }

            Selection.activeObject = targetBundleRef;
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

            if (!other.Name.Equals(Name)) return false;

            if (!Equals(other.targetBundleRef, targetBundleRef))
                return false;


            if (!other.overrideValues.Equals(overrideValues)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
       
        public Bundle GetBundleRef()
        {
            if (!targetBundleRef) // if it's null load default
            {
                targetBundleRef = LBSAssetMacro.LoadAssetByGuid<Bundle>(defaultBundleGuid);
            }
            
            return targetBundleRef;
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