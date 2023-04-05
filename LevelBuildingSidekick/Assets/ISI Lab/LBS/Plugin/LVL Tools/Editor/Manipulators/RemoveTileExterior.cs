using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTileExterior<T> : ManipulateTeselation<T> where T : LBSTile
{
    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        /*
        OnManipulationStart?.Invoke();
        var view = e.target as ExteriorTileView;
        
        if (view == null)
        {
            return;
        }
        var tile = view.Data;

        module.RemoveTile(tile as T);

        OnManipulationEnd?.Invoke();
        */
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position,  MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = MainView.ToTileCords(Vector2Int.Min(StartPosition, EndPosition));
        var max = MainView.ToTileCords(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var tile = module.GetTile(pos);
                module.RemoveTile(tile as T);
            }
        }
    }
}
