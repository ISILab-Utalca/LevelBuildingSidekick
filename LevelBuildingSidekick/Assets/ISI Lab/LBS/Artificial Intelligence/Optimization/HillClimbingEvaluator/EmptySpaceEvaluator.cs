using Commons.Optimization.Evaluator;
using LBS.Graph;
using LBS.Representation.TileMap;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EmptySpaceEvaluator : IEvaluator
{

    public EmptySpaceEvaluator() { }

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
        var graphData = evaluable as GraphicsModule;
        var schema = Heu as LBSSchemaData;
        var value = 0f;
        foreach (var room in schema.GetRooms())
        {
            var rectArea = room.GetRect().width * room.GetRect().height;
            var tc = room.TilesCount;
            value += 1 - (MathF.Abs(rectArea - tc) / (tc * 1f));
        }

        return value / (schema.RoomCount * 1f);
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

}