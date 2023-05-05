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
        var min = MainView.ToTileCords(Vector2Int.Min(StartPosition, EndPosition));
        var max = MainView.ToTileCords(Vector2Int.Max(StartPosition, EndPosition));

        var tiles = LBSAssetsStorage.Instance.Bundles
            .Select(b =>  b.GetCharacteristic<LBSDirection>())
            .ToList();

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

    public void CalculateTile(ConnectedTile tile, List<LBSDirection> connections)
    {
        var candidates = new List<Tuple<string[], LBSDirection>>();

        foreach (var connection in connections)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Compare(tile.Connections, connection.GetConnection(i)))
                {
                    candidates.Add(new Tuple<string[], LBSDirection>(connection.GetConnection(i), connection));
                }
            }
        }

        if (candidates.Count <= 0)
        {
            Debug.LogWarning("[ISI Lab]: No valid candidates found.");
            return;
        }


        var totalW = candidates.Sum(c => c.Item2.TotalWeight);
        var rulleteValue = UnityEngine.Random.Range(0, totalW);

        var index = -1;
        var cur = 0f;
        for (int i = 0; i < candidates.Count; i++)
        {
            cur += candidates[i].Item2.TotalWeight;
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
