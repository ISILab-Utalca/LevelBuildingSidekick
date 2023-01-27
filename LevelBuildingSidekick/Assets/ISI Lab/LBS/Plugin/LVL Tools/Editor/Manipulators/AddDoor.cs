using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using LBS.ElementView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Representation;
using LBS.Representation.TileMap;
using UnityEditor.PackageManager.UI;

public class AddDoor<T, U> : ManipulateTiledArea<T, U> where T : TiledArea<U> where U : LBSTile
{
    private List<Vector2Int> tiles;
    private TileData first;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        tiles = new List<Vector2Int>();

        var tile = e.target as LBSTileView;
        if (tile == null)
            return;

        first = tile.Data;
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
       
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var tile = e.target as LBSTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        var r1 = module.GetRoomPos(first.Position);
        var r2 = module.GetRoomPos(second.Position);
        if (r1.Equals(r2))
            return;

        var dx = Mathf.Abs(first.Position.x - second.Position.x);
        var dy = Mathf.Abs(first.Position.y - second.Position.y);
        
        if (dx + dy > 1f)
            return;

        var door = new DoorData(
            first.Position,
            second.Position
            );

        OnManipulationEnd?.Invoke();
    }

}
