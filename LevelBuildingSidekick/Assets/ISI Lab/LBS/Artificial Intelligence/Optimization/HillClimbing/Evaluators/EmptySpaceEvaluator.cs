using Commons.Optimization.Evaluator;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Graph;
using LBS.Components.TileMap;
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
        var graphData = evaluable as LBSRoomGraph;
        var schema = Heu as LBSSchema;
        var value = 0f;
        foreach (var room in schema.Areas)
        {
            var rectArea = room.Rect.width * room.Rect.height;
            var tc = room.TileCount;
            value += 1 - (MathF.Abs(rectArea - tc) / (tc * 1f));
        }

        return value / (schema.RoomCount * 1f);
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

}