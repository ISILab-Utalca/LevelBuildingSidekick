using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtGrid<T,U> : LBSManipulator where T : TiledArea<U> where U : LBSTile
{
    private AreaTileMap<T,U> module;
    private MainView mainView;

    private Vector2Int startPos;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<AreaTileMap<T,U>>();
        this.mainView = view;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var fixPos = mainView.FixPos(e.localMousePosition);
        startPos = mainView.ToTileCords(fixPos);
    }

    private void OnMouseMove(MouseMoveEvent e)
    {

    }

    private void OnMouseUp(MouseUpEvent e)
    {
        var tPos1 = startPos;
        var fixPos = mainView.FixPos(e.localMousePosition);
        var tPos2 = mainView.ToTileCords(fixPos);

        /*

        for (int i = tPos1.y; i <= tPos2.y; i++)
        {
            for (int j = tPos1.x; j <= tPos2.x; j++)
            {
                var x = Activator.CreateInstance(typeof(T)) as T;
                var tile = new TileData(new Vector2Int(j, i), 0, new string[4]); // (!) esto solo esta para 4 conectados
                schema.AddTile(tile, cRoom.ID);
                window.RefreshView();
            }
        }
        */
        Debug.LogWarning("[LBS]: Funcion no implemntada");
        OnManipulationEnd?.Invoke();
    }
}
