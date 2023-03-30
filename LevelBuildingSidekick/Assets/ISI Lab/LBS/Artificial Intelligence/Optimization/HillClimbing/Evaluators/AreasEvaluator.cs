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
        var DeltaW = node.Room.Width;
        var DeltaH = node.Room.Height;
        var vw = 1f;
        if (room.Width < DeltaW - delta.x || room.Width > DeltaW + delta.x)
        {
            vw -= (Mathf.Abs(node.Room.Width - room.Width) / ((float)node.Room.Width * 1f));
        }

        var vh = 1f;
        if (room.Height < DeltaH - delta.y || room.Height > DeltaH + delta.y)
        {
            vh -= (Mathf.Abs(node.Room.Height - room.Height) / ((float)node.Room.Height * 1f));
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
