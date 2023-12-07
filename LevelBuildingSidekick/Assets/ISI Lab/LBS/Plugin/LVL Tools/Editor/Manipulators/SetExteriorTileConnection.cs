using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using UnityEngine.UIElements;
using LBS;
using NUnit;

public class SetExteriorTileConnection : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;

    private LBSIdentifier toSet;
    private ExteriorBehaviour exterior;
    private Vector2Int first;

    private ConnectedConrnerLine lineFeedback = new ConnectedConrnerLine();
    private Feedback areaFeedback = new AreaFeedback();

    public LBSIdentifier ToSet
    {
        get => toSet;
        set => toSet = value;
    }

    public SetExteriorTileConnection() : base()
    {
        lineFeedback.fixToTeselation = true;
        areaFeedback.fixToTeselation = true;
        feedback = lineFeedback;
    }

    public override void Init(LBSLayer layer, object behaviour)
    {
        exterior = behaviour as ExteriorBehaviour;
        lineFeedback.TeselationSize = layer.TileSize;
        areaFeedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) =>
        {
            lineFeedback.TeselationSize = val;
            areaFeedback.TeselationSize = val;
        };
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = exterior.Owner.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        lineFeedback.LeftSide = e.shiftKey;

        if (!e.ctrlKey)
        {
            this.SetFeedback(lineFeedback);
            //feedback = lineFeedback;
            //feedback.ActualizePositions(first, exterior.Owner.ToFixedPosition(movePosition));
        }
        else
        {
            this.SetFeedback(areaFeedback);
            //feedback = areaFeedback;
            //feedback.ActualizePositions(first, exterior.Owner.ToFixedPosition(movePosition));
        }
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        if(toSet == null || toSet.Label == "")
        {
            Debug.LogWarning("NO tienes ninguna conexion seleccionada");
            return;
        }

        // Get end position
        var end = exterior.Owner.ToFixedPosition(position);

        if(!e.ctrlKey)
        {
            LineEffect(end, e);
        }
        else
        {
            AreaEffect(end, e);
        }

        /*
        // Get tile in first position
        var t1 = exterior.GetTile(first);

        // Cheack if valid
        if (t1 == null)
            return;

        // Get second fixed position
        var pos = exterior.Owner.ToFixedPosition(position);

        // Get vector direction
        var dx = (t1.Position.x - pos.x);
        var dy = (t1.Position.y - pos.y);

        // Get index of direction
        var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= exterior.Directions.Count)
            return;

        var t2 = exterior.GetTile(pos);
        if (t2 == null)
        {
            exterior.SetConnection(t1, fDir, toSet.Label, false);
            return;
        }

        if (t1.Equals(t2))
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = exterior.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        // Cheack if label is null
        if(toSet == null || toSet.Label == "")
        {
            Debug.LogWarning("NO tienes ninguna conexion seleccionada");
            return;
        }

        exterior.SetConnection(t1, fDir, toSet.Label, false);
        exterior.SetConnection(t2, tDir, toSet.Label, false);
        */
    }

    public void LineEffect(Vector2Int end, MouseUpEvent e)
    {
        // Get corner position
        var corner = e.shiftKey ?
            new Vector2Int(first.x, end.y) :
            new Vector2Int(end.x, first.y);

        List<(LBSTile, Vector2Int, Vector2Int)> path = new();

        // Get first path
        Vector2Int current = first;
        while (!current.Equals(corner))
        {
            var tile = exterior.GetTile(current);
            var dir = ((Vector2)(corner - first)).normalized.ToInt();

            path.Add((tile, new Vector2Int(current.x, current.y), dir));
            current += dir;
        }

        // Get second path
        current = corner;
        while (!current.Equals(end))
        {
            var tile = exterior.GetTile(current);
            var dir = ((Vector2)(end - corner)).normalized.ToInt();

            path.Add((tile, new Vector2Int(current.x, current.y), dir));
            current += dir;
        }

        for (int i = 0; i < path.Count; i++)
        {
            var t1 = path[i].Item1;
            var fDir = Directions.FindIndex(d => d.Equals(path[i].Item3));

            if (t1 != null)
            {
                exterior.SetConnection(t1, fDir, toSet.Label, false);
            }

            var t2 = exterior.GetTile(path[i].Item2 + path[i].Item3);
            var dDir = Directions.FindIndex(d => d.Equals(-path[i].Item3));

            if (t2 != null)
            {
                exterior.SetConnection(t2, dDir, toSet.Label, false);
            }
        }
    }

    public void AreaEffect(Vector2Int end, MouseUpEvent e)
    {
        var corners = exterior.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var tile = exterior.GetTile(pos);

                if(tile == null)
                {
                    continue;
                }

                for (int k = 0; k < Directions.Count; k++)
                {
                    exterior.SetConnection(tile, k, toSet.Label, false);

                    var dir = Directions[k];
                    var neig = exterior.GetTile(pos + dir);

                    if(neig != null)
                    {
                        exterior.SetConnection(neig, (k + 2) % 4, toSet.Label, false);
                    }
                }
            }
        }
    }
}

