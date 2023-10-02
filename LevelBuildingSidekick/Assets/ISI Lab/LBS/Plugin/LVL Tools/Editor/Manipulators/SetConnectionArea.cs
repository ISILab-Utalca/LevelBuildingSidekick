using LBS;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

// esta es una herramienta del behaviour de exterior !!!
public class SetConnectionArea : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;

    private LBSIdentifier toSet;
    private ExteriorBehaviour exterior;

    public override void Init(LBSLayer layer, object provider)
    {
        exterior = provider as ExteriorBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    public SetConnectionArea() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        /*
        if (toSet == null)
        {
            Debug.LogWarning("No tienens ninguna zona seleccionada para colocar.");
            return;
        }

        var corners = exterior.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                var tile = exteriro
                exterior.AddTile(tile);

                for (int k = 0; k < Directions.Count; k++)
                {
                    exterior.SetConnection(tile, k, toSet.Label, false);
                }
            }
        }
        */
    }
}
