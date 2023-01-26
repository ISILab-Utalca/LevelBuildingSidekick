using Commons.Optimization.Evaluator;
using LBS.Graph;
using LBS.Representation.TileMap;
using LBS.Schema;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AreasEvaluator : IEvaluator
{
    public AreasEvaluator() { }

    public float EvaluateH<u>(IEvaluable evaluable, u Heu)
    {
        var graphData = evaluable as GraphicsModule;
        var schema = Heu as LBSSchemaData;
        var value = 0f;
        for (int i = 0; i < graphData.NodeCount(); i++)
        {
            var node = graphData.GetNode(i) as RoomCharacteristicsData;
            var room = schema.GetRoom(node.Label);
            switch (node.ProportionType)
            {
                case ProportionType.RATIO:
                    value += EvaluateByRatio(node, room);
                    break;
                case ProportionType.SIZE:
                    value += EvaluateBySize(node, room);
                    break;
            }
        }
        return value / (schema.RoomCount * 1f);
    }

    private float EvaluateByRatio(RoomCharacteristicsData node, RoomData room)
    {
        float current = room.Ratio;
        float objetive = node.AspectRatio.width / (float)node.AspectRatio.heigth;

        return 1 - (Mathf.Abs(objetive - current) / (float)objetive);
    }

    private float EvaluateBySize(RoomCharacteristicsData node, RoomData room)
    {
        var vw = 1f;
        if (room.Width < node.RangeWidth.min || room.Width > node.RangeWidth.max)
        {
            var objetive = node.RangeWidth.Middle;
            var current = room.Width;
            vw -= (Mathf.Abs(objetive - current) / (float)objetive);
        }

        var vh = 1f;
        if (room.Height < node.RangeHeight.min || room.Height > node.RangeHeight.max)
        {
            var objetive = node.RangeHeight.Middle;
            var current = room.Height;
            vh -= (Mathf.Abs(objetive - current) / (float)objetive);
        }

        return (vw + vh) / 2f;
    }

    public float Evaluate(IEvaluable evaluable)
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
