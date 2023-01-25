using LBS.Graph;
using LBS.Representation.TileMap;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AdjacenciesEvaluator
{   

    public AdjacenciesEvaluator() {}

    public float Run(LBSGraphData graphData, LBSSchemaData schema)
    {
        if (graphData.EdgeCount() <= 0)
        {
            Debug.LogWarning("Cannot calculate the adjacency of a map are nodes that are not connected.");
            return 0;
        }

        var distValue = 0f;
        for (int i = 0; i < graphData.EdgeCount(); i++)
        {
            var edge = graphData.GetEdge(i);

            var r1 = schema.GetRoom(edge.FirstNodeLabel);
            var r2 = schema.GetRoom(edge.SecondNodeLabel);

            var roomDist = GetRoomDistance(r1, r2);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija (?)
            if (roomDist <= 1)
            {
                distValue++;
            }
            else
            {
                var c = r1.TilesCount;
                var max1 = (r1.Width + r1.Height) / 2f;
                var max2 = (r2.Width + r2.Height) / 2f;
                distValue += 1 - (roomDist / (max1 + max2));
            }
        }

        return distValue / (float)graphData.EdgeCount();
    }

    private int GetRoomDistance(RoomData r1, RoomData r2) // O2 - manhattan
    {
        var lessDist = int.MaxValue;
        var tPos1 = r1.TilesPositions;
        var tPos2 = r2.TilesPositions;
        for (int i = 0; i < tPos1.Count; i++)
        {
            for (int j = 0; j < tPos2.Count; j++)
            {
                var t1 = tPos1[i];
                var t2 = tPos2[j];

                var dist = Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y); // manhattan

                if (dist <= lessDist)
                {
                    lessDist = dist;
                }
            }
        }
        return lessDist;
    }
}
