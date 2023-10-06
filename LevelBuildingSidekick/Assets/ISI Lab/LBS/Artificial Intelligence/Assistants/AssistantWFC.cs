using LBS.Assisstants;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[RequieredModule(
    typeof(TileMapModule),
    typeof(ConnectedTileMapModule)
    )]
public class AssistantWFC : LBSAssistant
{
    #region FIELDS
    [SerializeField, JsonRequired]
    private bool overrideValues;

    [SerializeField, JsonRequired]
    private string targetBundle = "Exterior_Plains";
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
        get => GetBundle(targetBundle);
        set => targetBundle = value.name;
    }
    #endregion

    #region CONSTRUCTORS
    public AssistantWFC(Texture2D icon, string name) : base(icon, name){ }
    #endregion

    #region METHODS
    public override object Clone()
    {
        return new AssistantWFC(this.Icon,this.Name);
    }

    public override void Execute()
    {
        // Get Bundle
        var bundle = GetBundle(targetBundle);

        // Cheack if can execute
        if(bundle == null)
            return;

        // Get bundles posible tiles
        var group = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];
        var connectionsChars = group.Connections;

        // Get modules
        var map = Owner.GetModule<TileMapModule>();
        var connected = Owner.GetModule<ConnectedTileMapModule>();

        // Get tiles to change
        var toCalc = new List<LBSTile>();
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

            toCalc.Add(tile);
        }

        var closed = new List<LBSTile>(); 

        // Run as long as you have tiles 
        while (toCalc.Count > 0)
        {
            // Recalculate posibilities for each tile
            var currentCalcs = new List<Tuple<LBSTile,List<Candidate>>>();
            foreach (var tile in toCalc)
            {
                // Get candidates related to current tile
                var candidates = CalcCandidates(tile, group);

                var c = new Tuple<LBSTile, List<Candidate>>(tile, candidates);
                currentCalcs.Add(c);
            }

            // Get tile with lees possibilities
            var current = currentCalcs.OrderBy(e => e.Item2.Count).First();

            // cheack if curren tile have tile posibilities
            if (currentCalcs.Count <= 0)
            {
                // Remove from the list of tiles to calculate 
                Debug.Log(current.Item1.Position + " no tiene posibles tile.");
                toCalc.Remove(current.Item1);
                continue;
            }

            // Collapse posibilities
            var selected = current.Item2.RandomRullete(c => c.weigth);
            var connections = selected.bundle.GetConnection(selected.rotation);
            connected.SetConnections(current.Item1, connections.ToList(), new List<bool>() { false, false, false, false });

            // Get direction
            var dirs = Directions.Bidimencional.Edges;

            // Ignore This tiles
            closed.Add(current.Item1);

            // Collapse neigthbours connection 
            var neigth = map.GetTileNeighbors(current.Item1, dirs);
            SetConnectionNei(current.Item1, neigth.ToArray(), closed);

            // Remove from the list of tiles to calculate 
            toCalc.Remove(current.Item1);
        }
    }

    private List<Candidate> CalcCandidates(LBSTile tile, LBSDirectionedGroup group)
    {
        // Get modules
        var connectedMod = Owner.GetModule<ConnectedTileMapModule>();

        var candidates = new List<Candidate>();
        for (int i = 0; i < group.Weights.Count; i++)
        {
            // Get characteristics and weigth
            var weigth = group.Weights[i].weigth;
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

        if(!other.Name.Equals(this.Name)) return false;

        if(!other.targetBundle.Equals(this.targetBundle)) return false;

        if(!other.overrideValues.Equals(this.overrideValues)) return false;

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