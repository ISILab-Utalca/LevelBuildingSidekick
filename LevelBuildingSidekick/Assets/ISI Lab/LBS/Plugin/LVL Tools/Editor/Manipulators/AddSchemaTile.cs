using LBS;
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
        if(e.ctrlKey)
        {
            var newZone = schema.AddZone();
            newZone.InsideStyles = new List<string>() { schema.PressetInsideStyle.Name };
            newZone.OutsideStyles = new List<string>() { schema.PressetOutsideStyle.Name };

            toSet = newZone;
        }

        if (toSet == null)
        {
            Debug.LogWarning("No tienens ninguna zona seleccionada para colocar.");
            return;
        }

        var corners = schema.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
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

