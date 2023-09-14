using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RotatePopulationTile : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;

    PopulationBehaviour population;
    private Vector2Int first;

    public RotatePopulationTile()
    {

        feedback = new ConnectedLine();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object provider)
    {
        population = provider as PopulationBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        first = population.Owner.ToFixedPosition(startPosition);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var pos = population.Owner.ToFixedPosition(endPosition);

        var dx = (first.x - pos.x);
        var dy = (first.y - pos.y);

        var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
        //var fDir = schema.Connections.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= Directions.Count)
            return;

        population.RotateTile(first, Directions[fDir]);
    }
}
