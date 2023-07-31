using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class WaveFunctionCollapseManipulator : ManipulateTeselation
{
    private struct Candidate
    {
        public string[] array;
        public float weight;
    }


    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
    };

    private ConnectedTile first;

    public WaveFunctionCollapseManipulator() : base()
    {

    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = this.module.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = this.module.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
        var fathers = bundles
            .Select(b => b.GetCharacteristic<LBSDirectionedGroup>())    // obtiene todos los bundles que tengan DirsGroup
            .Where(e => e != null)                                      // que no sean nulls
            .Where(e => !e.Owner.isPreset)                              // ni pressets;
            .ToList();

        var father = fathers[0];

        if(fathers.Count <= 0)
        {
            Debug.Log("[ISI Lab]: There are no structured bundles available for this tool.");
            return;
        }

        var toCalc = new List<ConnectedTile>();
        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var current = module.GetTile(new Vector2Int(i, j)) as ConnectedTile;
                if (current == null)
                    continue;

                if (e.ctrlKey)
                {
                    current.SetConnections(new string[4] { "", "", "", "" });
                }
                toCalc.Add(current);
            }
        }

        while (toCalc.Count > 0)
        {
            var currentCalcs = new List<Tuple<ConnectedTile, List<Candidate>>>();
            foreach (var tile in toCalc)
            {
                var candidates = CalcCandidates(tile, father);

                var cTile = new Tuple<ConnectedTile, List<Candidate>>(tile, candidates);
                currentCalcs.Add(cTile);
            }

            var current = currentCalcs.OrderBy(e => e.Item2.Count).First();

            if (currentCalcs.Count <= 0)
            {
                toCalc.Remove(current.Item1);
                continue;
            }

            // collapse
            var selected = current.Item2.RandomRullete(c => c.weight);
            current.Item1.SetConnections(selected.array);

            //var neigs = module.GetTileNeighbors(current.Item1 as T, dirs).Select(t => t as ConnectedTile);
            //SetConnectionNei(current.Item1.Connections,neigs.ToArray());

            toCalc.Remove(current.Item1);
        }

    }

    private List<Candidate> CalcCandidates(ConnectedTile tile, LBSDirectionedGroup group)
    {
        var candidates = new List<Candidate>();

        for (int i = 0; i < group.Weights.Count; i++)
        {
            var weigth = group.Weights[i].weigth;
            var sBundle = group.Weights[i].target.GetCharacteristic<LBSDirection>();
            for (int j = 0; j < 4; j++) // esto deberia ser por numero de conexiones y no directamente un 4 (!!)
            {
                var array = sBundle.GetConnection(j);
                if (Compare(tile.Connections, array))
                {
                    var c = new Candidate()
                    {
                        array = array,
                        weight = weigth,
                    };

                    candidates.Add(c);
                }
            }
        }

        return candidates;
    }

    public void SetConnectionNei(string[] oring, ConnectedTile[] neis)
    {
        for (int i = 0; i < neis.Length; i++)
        {
            if (neis[i] == null)
                continue;

            var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));
            neis[i].SetConnection(oring[i], idir);
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
}
