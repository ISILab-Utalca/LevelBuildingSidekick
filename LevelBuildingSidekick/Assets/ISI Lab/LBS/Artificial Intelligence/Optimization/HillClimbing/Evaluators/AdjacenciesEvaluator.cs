using Commons.Optimization.Evaluator;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Components.Graph;
using LBS.Components.Specifics;
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

        float distValue = 0f;
        for (int i = 0; i < graph.EdgeCount; i++)
        {
            var edge = graph.GetEdge(i);

            var r1 = schema.GetArea(edge.FirstNode.ID);
            var r2 = schema.GetArea(edge.SecondNode.ID);

            if (r1.TileCount < 1 || r2.TileCount < 1) // signiofica que una de las dos areas desaparecio y no deberia aporta, de hecho podria ser negativo (!)
                continue;

            float roomDist = schema.GetRoomDistance(r1.ID, r2.ID);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija (?)

            distValue += 1 / roomDist;
            /*if (roomDist <= 1)
            {
                distValue++;
            }
            else
            {
                var widthAverage = (r1.Width + r2.Width) / 2f;
                var heightAverage = (r1.Height + r2.Height) / 2f;

                distValue += 1 - (roomDist / (widthAverage + heightAverage));
            }*/
        }

        return distValue / graph.EdgeCount;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
