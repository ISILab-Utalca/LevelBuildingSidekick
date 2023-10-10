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
    private SectorizedTileMapModule zones;
    private ConnectedZonesModule connectedZones;

    public AdjacenciesEvaluator() {}

    public AdjacenciesEvaluator(LBSLayer layer) 
    {
        this.zones = layer.GetModule<SectorizedTileMapModule>();
        this.connectedZones = layer.GetModule<ConnectedZonesModule>();
    }

    public object Clone()
    {
        throw new System.NotImplementedException();
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var layer = (evaluable as OptimizableModules).Modules;
        var connectedZones = layer.GetModule<ConnectedZonesModule>();
        var zones = layer.GetModule<SectorizedTileMapModule>();

        var edgeCount = connectedZones.Edges.Count;
        if (edgeCount <= 0)
        {
            Debug.Log("Cannot calculate the adjacency of a map are nodes that are not connected.");
            return 1;
        }

        if(zones.ZonesWithTiles.Count <= 0)
        {
            Debug.Log("[ISI Lab]: the schema you are trying to evaluate does not have areas.");
            return 0;
        }

        float distValue = 0f;
        for (int i = 0; i < edgeCount; i++)
        {
            var edge = connectedZones.Edges[i];

            var r1 = zones.GetTiles(edge.First); //schema.GetArea(edge.FirstNode.ID);
            var r2 = zones.GetTiles(edge.Second); //schema.GetArea(edge.SecondNode.ID);

            if (r1.Count < 1 || r2.Count < 1) // signiofica que una de las dos areas desaparecio y no deberia aporta, de hecho podria ser negativo (!)
                continue;

            float roomDist = zones.GetRoomDistance(edge.First, edge.Second);  // este metodo podria recivir una funcion de calculo de distancia en ved de estar fija 

            distValue += 1 / roomDist;
        }

        if(edgeCount <= 0)
        {
            Debug.Log("E: " + edgeCount);
            return 0;
        }

        return distValue / edgeCount;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
