using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class PoligonAreaFeedback : GraphElement
{
    private Collider2D polygon = new Collider2D(); // dont work

    private Vector2Int teselationSize = new Vector2Int(100, 100);

    public Vector2Int TeselationSize
    {
        get => teselationSize;
        set => teselationSize = (LBSSettings.Instance.general.TileSize * value).ToInt();
    }

    public void ActualizePositions(List<Vector2Int> cords)
    {
        polygon = new Collider2D(); // dont work

        foreach (var cord in cords)
        {
            var p = new Collider2D();

            var points = new List<Vector2>();
            points.Add(cord + (new Vector2(1, 1) * teselationSize));
            points.Add(cord + (new Vector2(-1, 1) * teselationSize));
            points.Add(cord + (new Vector2(1, -1) * teselationSize));
            points.Add(cord + (new Vector2(-1, -1) * teselationSize));

            //p. SetPath(0, points);

            //polygon.
        }
    }

    protected void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        throw new System.NotImplementedException();
    }

    protected Vector2Int CalcFixTeselation(Vector2Int position)
    {
        var spX = (position.x >= 0) ?
                (position.x / TeselationSize.x) :
                (position.x / TeselationSize.x) - 1;
        var spY = (position.y >= 0) ?
            (position.y / TeselationSize.y) :
            (position.y / TeselationSize.y) - 1;
        return new Vector2Int(spX, spY);
    }

    public enum State
    {
        Correct = 0,
        Problem = 1,
        Error = 2
    }
}
