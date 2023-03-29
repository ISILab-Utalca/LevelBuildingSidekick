using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.AI;
using UnityEngine.UIElements;
using LBS.Components;
using LBS.Components.TileMap;
using System.Linq;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Populations;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using LBS.Components.Graph;
using LBS.Tools.Transformer;

[System.Serializable]
public class SchemaHCAgent : LBSAIAgent
{
    HillClimbing hillClimbing;

    public override void Execute()
    {
        OnStart?.Invoke();

        this.Init(ref this.layer);

        hillClimbing.Run();
        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        CalculateConnections.Operate(x);

        layer.SetModule<LBSSchema>(x , x.Key);

        OnTermination?.Invoke();

    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(ref LBSLayer layer)
    {
        name = "Schema HillClimbing";

        OnStart += () => Debug.Log("HillClimbing start!");
        OnTermination += () => Debug.Log("HillClimbing finish!");

        this.layer = layer;

        var graph = layer.GetModule<LBSRoomGraph>();

        var schema = layer.GetModule<LBSSchema>();
        var adam = new OptimizableSchema(schema);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(100); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluación
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(graph), 0.5f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(graph), 0.35f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(), 0.15f),

        }) ;
        var population = new Population(1, 100, adam); // agregar parametros

        hillClimbing = new HillClimbing(population, evaluator, selection, GetNeighbors, termination); // asignar Adam

    }

    public List<IOptimizable> GetNeighbors(IOptimizable Adam)
    {
        var tileMap = (Adam as OptimizableSchema).Schema;
        var neighbours = new List<IOptimizable>();

        for (int i = 0; i < tileMap.AreaCount; i++)
        {
            var area = tileMap.GetArea(i);
            var vWalls = area.GetVerticalWalls();
            var hWalls = area.GetHorizontalWalls();
            var walls = vWalls.Concat(hWalls);

            // Add wall tiles in wall direction to the next gen
            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                //var area = neighbour.GetArea(i);
                wall.Tiles.ForEach(t => neighbour.AddTile(area.ID, new ConnectedTile(t + wall.Dir, area.ID, 4)));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            // remove wall to the next gen
            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                //var area = neighbour.GetArea(i);
                wall.Tiles.ForEach(t => neighbour.RemoveTile(t));

                neighbours.Add(new OptimizableSchema(neighbour));
            }
        }

        return neighbours;
    }
}
