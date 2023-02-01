using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtLine<T,U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    private Vector2Int startPos;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var fixPos = mainView.FixPos(e.localMousePosition);
        startPos = mainView.ToTileCords(fixPos);
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        var fixPos = mainView.FixPos(e.localMousePosition);
        var endPos = mainView.ToTileCords(fixPos);

        var positons = CalcTiles(startPos, endPos);
        foreach (var pos in positons)
        {
            var tile = Activator.CreateInstance(typeof(U)) as U;
            tile.Position = pos;
            areaToSet?.AddTile(tile);
        }

        OnManipulationEnd?.Invoke();
    }

    private List<Vector2Int> CalcTiles(Vector2Int startPos, Vector2Int endPos)
    {
        var toR = new List<Vector2Int>();
        toR.Add(startPos);
        var current = new Vector2Int(startPos.x, startPos.y);
        while (current.Equals(endPos))
        {
            var nei = current.Get4Connected();
            float lessDist = Vector2.Distance(current, endPos);
            foreach (var n in nei)
            {
                var dist = Vector2.Distance(n, endPos);
                if (dist < lessDist)
                {
                    lessDist = dist;
                    current = n;
                }
            }
            toR.Add(current);
        }
        toR.Add(endPos);

        return toR;
    }

}
