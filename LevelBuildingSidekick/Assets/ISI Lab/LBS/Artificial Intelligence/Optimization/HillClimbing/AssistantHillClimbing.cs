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
using System;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;

[System.Serializable]
[RequieredModule(typeof(LBSRoomGraph), typeof(LBSSchema))]
/*[Metadata(
    "Hill climbing algorithm",
    "THIS/IS/A/FAKE/PATH/ICON", // Implementar bien (!!!)
    "Hill Climbing is an optimization algorithm that iteratively improves a" +
    " solution by exploring neighboring options.")]*/
public class AssistantHillClimbing : LBSAssistantAI
{
    [JsonIgnore, NonSerialized]
    private HillClimbing hillClimbing;
    [JsonIgnore, NonSerialized]
    private Stopwatch clock = new Stopwatch();

    [NonSerialized]
    private LBSLayer layer;

    public AssistantHillClimbing() {}
    /*
    public SchemaHCAgent(LBSLayer layer, string id) : base(layer, id, "SchemaHillClimbing")
    {
    }
    */

    public override void Execute()
    {
        clock = new Stopwatch();

        UnityEngine.Debug.Log("HillClimbing start!");
        OnStart?.Invoke();

        clock.Start();
        hillClimbing.Start();
        clock.Stop();

        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        CalculateConnections.Operate(x);

        SetDoors(x, Owner.GetModule<LBSRoomGraph>());

        Owner.SetModule(x, x.Key);

        OnTermination?.Invoke();
        UnityEngine.Debug.Log("HillClimbing finish!");
    }

    public override VisualElement GetInspector()
    {
        UnityEngine.Debug.Log("No implemntado HillClimbing");
        return new Label("No implementado");
    }

    private void SetDoors(LBSSchema schema, LBSRoomGraph graph)
    {
        foreach (var area in schema.Areas)
        {
            foreach (var tile in area.Tiles)
            {
                var cTile = tile as ConnectedTile;
                for (int i = 0; i < cTile.Connections.Length; i++)
                {
                    if (cTile.Connections[i].Contains("Door"))
                    {
                        cTile.SetConnection("Wall", i);
                    }
                }
            }
        }

        for (int i = 0; i < graph.EdgeCount; i++)
        {
            var edge = graph.GetEdge(i);

            var r1 = schema.GetArea(edge.FirstNode.ID);
            var r2 = schema.GetArea(edge.SecondNode.ID);

            if (r1.TileCount <= 0 || r2.TileCount <= 0) // signiofica que una de las dos areas desaparecio y no deberia aporta, de hecho podria ser negativo (!)
                continue;

            var pairs = new List<Tuple<ConnectedTile, ConnectedTile>>();
            foreach (var t1 in r1.Tiles)
            {
                foreach (var t2 in r2.Tiles)
                {
                    var dist = Vector2Int.Distance(t1.Position, t2.Position);
                    if (dist <= 1.1f)
                    {
                        pairs.Add(new Tuple<ConnectedTile, ConnectedTile>(t1 as ConnectedTile, t2 as ConnectedTile));
                    }
                }
            }

            if (pairs.Count <= 0)
                continue;

            var selc = pairs.GetRandom();
            // var selc = pairs[UnityEngine.Random.Range(0, pairs.Count() - 1)];

            var dir = selc.Item1.Position - selc.Item2.Position;

            selc.Item1.SetConnection("Door", -dir);
            selc.Item2.SetConnection("Door", dir);

        }


    }

    public override void Init(LBSLayer layer)
    {
        var graph = layer.GetModule<LBSRoomGraph>();

        var schema = layer.GetModule<LBSSchema>();
        var adam = new OptimizableSchema(schema);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(5); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluaci�n
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(graph), 0.4f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(graph), 0.15f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(), 0.35f),
            new System.Tuple<IEvaluator, float> (new RoomCutEvaluator(graph), 1f),
            //new System.Tuple<IEvaluator, float> (new StretchEvaluator(), 0.1f),
        });
        var population = new Population(1, 100, adam); // agregar parametros

        hillClimbing = new HillClimbing(population, evaluator, selection, GetNeighbors, termination); // asignar Adam
    }

    public List<IOptimizable> GetNeighbors(IOptimizable Adam)
    {
        var tileMap = (Adam as OptimizableSchema).Schema;
        var neighbours = new List<IOptimizable>();

        //string s = "";

        for (int i = 0; i < tileMap.AreaCount; i++)
        {
            var area = tileMap.GetArea(i);
            var vWalls = area.GetVerticalWalls();
            var hWalls = area.GetHorizontalWalls();
            var walls = vWalls.Concat(hWalls).ToList();
            /*
            foreach(var e in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

            }*/

            // Add wall tiles in wall direction to the next gen
            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                wall.Tiles.ForEach(t => neighbour.AddTile(area.ID, new ConnectedTile(t + wall.Dir, area.ID, 4)));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            // Add wall tiles in wall direction contraria a to the next gen
            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                wall.Tiles.ForEach(t => neighbour.AddTile(area.ID, new ConnectedTile(t - wall.Dir, area.ID, 4)));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            // remove wall to the next gen
            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                wall.Tiles.ForEach(t => neighbour.RemoveTile(t));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            var dirs = new List<Vector2Int>() { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            foreach (var dir in dirs)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                neighbour.MoveArea(i, dir);

                neighbours.Add(new OptimizableSchema(neighbour));
            }
        }


        return neighbours;
    }

    public override object Clone()
    {
        return new AssistantHillClimbing();
    }

    public void RunExperiment()
    { 
        var elitistLog = SchemaHCLog.CreateInstance("SchemaHCLog") as SchemaHCLog;
        elitistLog.name = "Elitist Log";
        clock = new Stopwatch();
        int experimentCount = 20;
        int stochasticRuns = 10;

        var schema = Owner.GetModule<LBSSchema>().Clone() as LBSSchema;

        Init(Owner);
        hillClimbing.OnGenerationRan += () => {
            var log = new HCLog();
            log.result = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
            log.time = clock.ElapsedMilliseconds;
            log.evaluationTime = hillClimbing.Elog;
            log.neighborTime = hillClimbing.Nlog;
            log.neighborCount = hillClimbing.NNlog;
            log.bestFitness = hillClimbing.BestCandidate.Fitness;
            elitistLog.log.Add(log);
        };
        hillClimbing.Selection = new EliteSelection();
        clock.Restart();
        hillClimbing.Start();
        clock.Stop();

        for (int i = 1; i <= experimentCount; i++)
        {
            hillClimbing.Termination = new GenerationNumberTermination(i);
            clock.Restart();
            hillClimbing.Run();
            clock.Stop();
        }
        /*
        AssetDatabase.CreateAsset(elitistLog, "Assets/ElitistLog.asset" );
        AssetDatabase.SaveAssets();*/

        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        CalculateConnections.Operate(x);

        SetDoors(x, Owner.GetModule<LBSRoomGraph>());

        Owner.SetModule(x, x.Key);
        OnTermination?.Invoke();

        for (int j = 0; j < stochasticRuns; j++)
        {
            Owner.SetModule(schema, schema.Key);
            Init(Owner);
            hillClimbing.Selection = new StochasticElitistSelection();
            for (int i = 1; i <= experimentCount; i++)
            {
                hillClimbing.Termination = new GenerationNumberTermination(i);

            }
        }

    }
}
