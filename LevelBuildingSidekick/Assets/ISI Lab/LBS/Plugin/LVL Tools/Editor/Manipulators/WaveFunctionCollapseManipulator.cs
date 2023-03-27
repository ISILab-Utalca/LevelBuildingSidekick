using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveFunctionCollapseManipulator<T> : ManipulateTileMap<T> where T : LBSTile
{
    private AreaFeedback feedback = new AreaFeedback();
    private Vector2 firstClick;

    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
    };

    private ConnectedTile first;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var view = e.target as ExteriorTileView;
        if (view == null)
            return;

        first = view.Data;

        mainView.AddElement(feedback);
        feedback.fixToTeselation = true;
        firstClick = mainView.FixPos(e.localMousePosition);
        feedback.ActualizePositions(firstClick.ToInt(), firstClick.ToInt());
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
        if (firstClick != null)
        {
            var pos = mainView.FixPos(e.localMousePosition);
            feedback.ActualizePositions(firstClick.ToInt(), pos.ToInt());
        }
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        mainView.RemoveElement(feedback);

        if (first == null)
            return;

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        var min = Vector2Int.Min(first.Position, second.Position);
        var max = Vector2Int.Max(first.Position, second.Position);

        var tiles = Utility.DirectoryTools.GetScriptables<WFCBundle>();

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

        do
        {
            var t = toCalc[UnityEngine.Random.Range(0, toCalc.Count)];
            CalculateTile(t, tiles);
            toCalc.Remove(t);

        } while (toCalc.Count > 0);


        OnManipulationEnd?.Invoke();
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
