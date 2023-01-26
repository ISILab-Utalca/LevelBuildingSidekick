using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using LBS.ElementView;
using LBS.Representation.TileMap;
using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class AddDoor<T> : LBSManipulator where T : LBSTile
{
    private TileMapModule<T> module;
    private LBSTileMapController controller;
    private TileData first;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<TileMapModule<T>>();
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
        var tile = e.target as LBSTileView;
        if (tile == null)
            return;

        first = tile.Data;

    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        //Debug.Log("Move drag");
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var tile = e.target as LBSTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        var schema = controller.GetData() as LBSSchemaData;
        var r1 = schema.GetRoom(first.Position);
        var r2 = schema.GetRoom(second.Position);
        if (r1.Equals(r2))
            return;

        var dx = Mathf.Abs(first.GetPosition().x - second.GetPosition().x);
        var dy = Mathf.Abs(first.GetPosition().y - second.GetPosition().y);
        if (dx + dy > 1f)
            return;

        var door = new DoorData(
            first.GetPosition(),
            second.GetPosition()
            );

        controller.AddDoor(door);
        OnManipulationEnd?.Invoke();
    }
}
