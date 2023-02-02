using Commons.Optimization.Evaluator;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Graph;
using LBS.Representation.TileMap;
using LBS.Schema;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AreasEvaluator : IEvaluator
{
    public AreasEvaluator() { }

    public float EvaluateH<u>(IOptimizable evaluable, u Heu)
    {
        var graphData = evaluable as LBSRoomGraph;
        var schema = Heu as LBSSchema;
        var value = 0f;
        for (int i = 0; i < graphData.NodeCount; i++)
        {
            var node = graphData.GetNode(i);
            var room = schema.GetArea(node.ID);

            EvaluateBySize(node, room);
        }
        return value / (schema.RoomCount * 1f);
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
        throw new System.NotImplementedException();
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
