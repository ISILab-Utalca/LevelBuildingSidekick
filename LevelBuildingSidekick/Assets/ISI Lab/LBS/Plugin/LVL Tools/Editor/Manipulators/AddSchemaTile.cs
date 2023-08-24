using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddSchemaTile : LBSManipulator
{

    private Zone toSet;
    
    private SchemaBehaviour schema;

    public Zone ToSet
    {
        get => toSet;
        set => toSet = value;
    }

    public AddSchemaTile() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        schema = owner as SchemaBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        if(toSet == null)
        {
            Debug.LogWarning("No tienens ninguna zona seleccionada para colocar.");
            return;
        }

        var min = schema.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = schema.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var tile = schema.AddTile(new Vector2Int(i, j), toSet);
                schema.AddConnections(
                    tile,
                    new List<string>() { "", "", "", "" },
                    new List<bool> { true, true, true, true }
                    );
            }
        }

        schema.RecalculateWalls();
    }
}
