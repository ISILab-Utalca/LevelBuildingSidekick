using Commons.Optimization.Evaluator;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AreasEvaluator : IEvaluator
{
    LBSRoomGraph graph;

    Vector2 delta;

    public AreasEvaluator() { }
    public AreasEvaluator(LBSRoomGraph graph) 
    {
        this.graph = graph;
    }

    private float EvaluateBySize(RoomNode node, TiledArea room)
    {
        var vw = 1f;
        if (room.Width != node.Room.Width)
        {
            vw = room.Width / (float)node.Room.Width;
            if (vw > 1)
                vw = 1 / vw;
        }

        var vh = 1f;
        if (room.Height != node.Room.Height)
        {
            vh = room.Height / (float)node.Room.Height;
            if (vh > 1)
                vh = 1 / vh;
        }

        return (vw + vh) / 2f;
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var schema = (evaluable as OptimizableSchema).Schema;
        var value = 0f;
        for (int i = 0; i < graph.NodeCount; i++)
        {
            var node = graph.GetNode(i);
            var room = schema.GetArea(node.ID);

            value += EvaluateBySize(node, room);
        }
        return value / (schema.AreaCount * 1f);
    }



}
