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
    private readonly List<Vector2Int> directions = Directions.Bidimencional.Edges;

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

    public override void Init(LBSLayer layer, LBSBehaviour behaviour)
    {
        schema = behaviour as SchemaBehaviour;
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
                schema.AddConnections(tile, new List<string>() { "", "", "", "" });
            }
        }

        RecalculateWalls();

    }

    private void RecalculateWalls()
    {
        foreach (var tile in schema.Tiles)
        {
            var currZone = schema.GetZone(tile);

            var currConnects = schema.GetConnections(tile);
            var neigs = schema.GetTileNeighbors(tile, directions);

            for (int i = 0; i < directions.Count; i++)
            {

                if (neigs[i] == null)
                {
                    if(currConnects[i] != "Door")
                    {
                        schema.SetConnection(tile, i, "Wall");
                    }
                    continue;
                }

                var otherZone = schema.GetZone(neigs[i]);
                if(otherZone == currZone)
                {
                    

                    schema.SetConnection(tile, i, "Empty");
                }
                else
                {
                    if (currConnects[i] != "Door")
                    {
                        schema.SetConnection(tile, i, "Wall");
                    }
                }
            }
        }
    }
}
