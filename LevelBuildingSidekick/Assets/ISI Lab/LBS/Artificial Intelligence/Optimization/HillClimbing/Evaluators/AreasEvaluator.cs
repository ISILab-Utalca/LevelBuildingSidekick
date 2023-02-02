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

    public AreasEvaluator() { }
    public AreasEvaluator(LBSRoomGraph graph) 
    {
        this.graph = graph;
    }

    private float EvaluateBySize(RoomNode node, TiledArea room)
    {
        var DeltaW = node.Width * 0.35;
        var DeltaH = node.Height * 0.35;
        var vw = 1f;
        if (room.Width < node.Width - DeltaW || room.Width > node.Width + DeltaW)
        {
            vw -= (Mathf.Abs(node.Width - room.Width) / (float)node.Width);
        }

        var vh = 1f;
        if (room.Height < node.Height - DeltaH || room.Height > node.Height + DeltaH)
        {
            vh -= (Mathf.Abs(node.Height - room.Height) / (float)node.Height);
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

            EvaluateBySize(node, room);
        }
        return value / (schema.RoomCount * 1f);
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public VisualElement CIGUI()
    {
        throw new System.NotImplementedException();
    }

}
