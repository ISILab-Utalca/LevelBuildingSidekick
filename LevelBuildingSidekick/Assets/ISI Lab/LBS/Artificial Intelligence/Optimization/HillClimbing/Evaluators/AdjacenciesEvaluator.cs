using Commons.Optimization.Evaluator;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AdjacenciesEvaluator : IEvaluator
{
    LBSRoomGraph graph;

    public AdjacenciesEvaluator() {}

    public AdjacenciesEvaluator(LBSRoomGraph graph) 
    {
        this.graph = graph;
    }

    public VisualElement CIGUI()
    {
        throw new System.NotImplementedException();
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var schema = (evaluable as OptimizableSchema).Schema;

        if (graph.EdgeCount <= 0)
        {
            Debug.LogWarning("Cannot calculate the adjacency of a map are nodes that are not connected.");
            return 1;
        }

        var distValue = 0f;
        for (int i = 0; i < graph.EdgeCount; i++)
        {
            var edge = graph.GetEdge(i);

            var r1 = schema.GetArea(edge.FirstNode.ID);
            var r2 = schema.GetArea(edge.SecondNode.ID);

            if (r1.TileCount <= 0 || r2.TileCount <= 0) // signiofica que una de las dos areas desaparecio y no deberia aporta, de hecho podria ser negativo (!)
                continue;

            var roomDist = schema.GetRoomDistance(r1.ID, r2.ID);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija (?)
            if (roomDist <= 1)
            {
                distValue++;
            }
            else
            {
                //var c = r1.TileCount;
                var max1 = (r1.Width + r1.Height) / 2f;
                var max2 = (r2.Width + r2.Height) / 2f;

                distValue += 1 - (roomDist / (max1 + max2));
                //distValue += 1/roomDist;
            }
        }

        return distValue / (float)graph.EdgeCount;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
