using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTileExterior<T> : ManipulateTileMap<T> where T : LBSTile
{
    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var view = e.target as ExteriorTileView;
        var tile = view.Data;

        if (tile == null)
        {
            return;
        }

        module.RemoveTile(tile as T);

        OnManipulationEnd?.Invoke();
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        //throw new System.NotImplementedException();
    }
}
