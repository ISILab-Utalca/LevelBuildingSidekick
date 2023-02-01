using Commons.Optimization.Evaluator;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Graph;
using LBS.Representation.TileMap;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AdjacenciesEvaluator : IEvaluator
{   

    public AdjacenciesEvaluator() {}

    public VisualElement CIGUI()
    {
        throw new System.NotImplementedException();
    }

    public float Evaluate(IEvaluable evaluable)
    {
        throw new System.NotImplementedException();
    }

    public float EvaluateH<u>(IEvaluable evaluable, u Heu)
    {
        var graphData = evaluable as LBSRoomGraph;
        var schema = Heu as LBSSchema;

        if (graphData.EdgeCount <= 0)
        {
            Debug.LogWarning("Cannot calculate the adjacency of a map are nodes that are not connected.");
            return 0;
        }

        var distValue = 0f;
        for (int i = 0; i < graphData.EdgeCount; i++)
        {
            var edge = graphData.GetEdge(i);

            var r1 = schema.GetArea(edge.FirstNode.ID);
            var r2 = schema.GetArea(edge.SecondNode.ID);

            var roomDist = schema.GetRoomDistance(r1.ID, r2.ID);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija (?)
            if (roomDist <= 1)
            {
                distValue++;
            }
            else
            {
                var c = r1.TileCount;
                var max1 = (r1.Width + r1.Height) / 2f;
                var max2 = (r2.Width + r2.Height) / 2f;
                distValue += 1 - (roomDist / (max1 + max2));
            }
        }

        return distValue / (float)graphData.EdgeCount;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
