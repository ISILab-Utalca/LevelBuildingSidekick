using LBS.Graph;
using LBS.Representation.TileMap;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;
using Assets.ISI_Lab.LBS.Artificial_Intelligence.Optimization.HillClimbingEvaluator;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class WeighuedEvaluator : IEvaluator
{
    Func< LBSGraphData, LBSSchemaData> Adjacencies;


    public WeighuedEvaluator()
    {
        Adjacencies = new AdjacenciesEvaluator();
        Areas = new AreasEvaluator();
        Empty = new EmptySpaceEvaluator();
    }

    public VisualElement CIGUI()
    {
        throw new NotImplementedException();
    }

    public float Evaluate(IEvaluable evaluable)
    {
        throw new NotImplementedException();
    }

    public float EvaluateH<u>(IEvaluable evaluable, u Heu)
    {
        throw new NotImplementedException();
    }

    public float EvaluateMap(LBSSchemaData schemaData, LBSGraphData graphData)
    {

        var evaluations = new Tuple<Func<LBSGraphData, LBSSchemaData, float>, float>[]
        {
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(Adjacencies,0.5f),
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(Areas,0.3f),
                new Tuple<Func<LBSGraphData, LBSSchemaData, float>,float>(Empty,0.2f)
            //new Tuple<Func<LBSGraphData, LBSSchemaData, float>, float>(EvaluateRoomDistribution,0.1f)
        };

        var value = 0f;
        for (int i = 0; i < evaluations.Count(); i++)
        {
            var action = evaluations[i].Item1;
            var weight = evaluations[i].Item2;
            value += (float)action?.Invoke(graphData, schemaData) * weight;
        }

        return value;
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}