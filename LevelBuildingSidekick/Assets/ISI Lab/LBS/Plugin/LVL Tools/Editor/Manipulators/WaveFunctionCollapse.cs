using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveFunctionCollapse<T> : ManipulateTileMap<T> where T : LBSTile
{
    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };


    private ConnectedTile first;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var view = e.target as ExteriorTileView;
        if (view == null)
            return;

        first = view.Data;
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        var min = Vector2Int.Min(first.Position,second.Position);
        var max = Vector2Int.Max(first.Position, second.Position);

        var tiles = Utility.DirectoryTools.GetScriptables<WFCBundle>();

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var t = module.GetTile(new Vector2Int(i, j)) as ConnectedTile;
                CalculateTile(t,tiles);
            }
        }

        OnManipulationEnd?.Invoke();
    }

    public void CalculateTile(ConnectedTile tile, List<WFCBundle> wfcTiles)
    {
        foreach (var wfc in wfcTiles)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Compare(tile.Connections, wfc.GetConnection(i)))
                {
                    tile.SetConnections(wfc.GetConnection(i));
                    var neis = module.GetTileNeighbors(tile as T, dirs).Select(t => t as ConnectedTile);
                    SetConnectionNei(tile.Connections,neis.ToArray());
                    return;
                }
            }
        }
        
    }

    public void SetConnectionNei(string[] oring, ConnectedTile[] neis)
    {
        for (int i = 0; i < neis.Length; i++)
        {
            var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));
            neis[i]?.SetConnection(oring[i], idir);
        }
    }

    public bool Compare(string[] a,string[] b)
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
