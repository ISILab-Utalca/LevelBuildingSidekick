using LBS;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddPopulationTile : LBSManipulator
{
    Bundle toSet;

    PopulationBehaviour population;

    public Bundle ToSet
    {
        get => toSet;
        set => toSet = value;
    }

    public AddPopulationTile() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        population = owner as PopulationBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        if (toSet == null)
        {
            Debug.LogWarning("No tienens ninguna zona seleccionada para colocar.");
            return;
        }

        var corners = population.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                population.AddTile(new Vector2Int(i, j), toSet);
            }
        }
    }
}
