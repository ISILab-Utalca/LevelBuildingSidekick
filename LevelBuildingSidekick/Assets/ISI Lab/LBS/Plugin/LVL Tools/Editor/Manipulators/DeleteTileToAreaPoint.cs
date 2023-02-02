using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeleteTileToAreaPoint<T, U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var view = e.target as SchemaTileView;
        var tile = view.Data;

        if (tile == null)
        {
            return;
        }
        var area = module.GetArea(tile.Position);

        if(area != null)
        {
            area.RemoveTile(tile);
        }

        var parche = new AreaToTileMap(); // (!!!!!) eliminar!!!
        parche.ParcheDiParche(module);

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
