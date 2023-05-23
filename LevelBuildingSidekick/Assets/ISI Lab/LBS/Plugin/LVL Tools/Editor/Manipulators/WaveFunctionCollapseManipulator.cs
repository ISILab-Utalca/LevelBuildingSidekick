using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveFunctionCollapseManipulator<T> : ManipulateTeselation<T> where T : LBSTile
{
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

        var tiles = LBSAssetsStorage.Instance.Bundles
            .Where(b =>  b.GetType() == typeof(WFCBundle))
            .Select(b => b as WFCBundle)
            .ToList();
        //var tiles = Utility.DirectoryTools.GetScriptables<WFCBundle>();

        var toCalc = new List<ConnectedTile>();
        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var t = module.GetTile(new Vector2Int(i, j)) as ConnectedTile;
                if (t == null)
                    continue;
                toCalc.Add(t);
            }
        }

        while(toCalc.Count > 0)
        {
            var t = toCalc[UnityEngine.Random.Range(0, toCalc.Count -1)];

            if (e.ctrlKey)
                t.SetConnections(new string[4] {"", "", "", ""});

            CalculateTile(t, tiles);
            toCalc.Remove(t);
        } 
    }

    public void CalculateTile(ConnectedTile tile, List<WFCBundle> wfcTiles)
    {
        var candidates = new List<Tuple<string[], WFCBundle>>();

        foreach (var wfc in wfcTiles)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Compare(tile.Connections, wfc.GetConnection(i)))
                {
                    candidates.Add(new Tuple<string[], WFCBundle>(wfc.GetConnection(i), wfc));
                }
            }
        }

        if (candidates.Count <= 0)
        {
            Debug.LogWarning("[ISI Lab]: No valid candidates found.");
            return;
        }

        var totalW = candidates.Sum(c => c.Item2.weight);
        var rulleteValue = UnityEngine.Random.Range(0, totalW);

        var index = -1;
        var cur = 0f;
        for (int i = 0; i < candidates.Count; i++)
        {
            cur += candidates[i].Item2.weight;
            if (rulleteValue <= cur)
            {
                index = i;
                break;
            }
        }
        var candidate = candidates[index];

        //var candidate = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        tile.SetConnections(candidate.Item1);
        var neis = module.GetTileNeighbors(tile as T, dirs).Select(t => t as ConnectedTile);
        SetConnectionNei(tile.Connections, neis.ToArray());
    }

    public void SetConnectionNei(string[] oring, ConnectedTile[] neis)
    {
        for (int i = 0; i < neis.Length; i++)
        {
            var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));
            neis[i]?.SetConnection(oring[i], idir);
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
