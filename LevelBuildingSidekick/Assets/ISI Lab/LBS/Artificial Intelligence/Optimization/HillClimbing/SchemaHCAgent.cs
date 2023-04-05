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

[System.Serializable]
public class SchemaHCAgent : LBSAIAgent
{
    HillClimbing hillClimbing;

    public SchemaHCAgent(LBSLayer layer, string id): base(layer, id, "SchemaHillClimbing")
    {

    }

    public override void Execute()
    {
        Debug.Log("HillClimbing start!");
        OnStart?.Invoke();

        this.Init(ref this.layer);

        hillClimbing.Run();
        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        CalculateConnections.Operate(x);

        layer.SetModule<LBSSchema>(x , x.Key);

        SetDoors(layer.GetModule<LBSSchema>(), layer.GetModule<LBSRoomGraph>());

        OnTermination?.Invoke();
        Debug.Log("HillClimbing finish!");
    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
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
                    if(dist <= 1.1f)
                    {
                        pairs.Add(new Tuple<ConnectedTile, ConnectedTile>(t1 as ConnectedTile, t2 as ConnectedTile));
                    }
                }
            }

            var selc = pairs[UnityEngine.Random.Range(0, pairs.Count() - 1)];

            var dir = selc.Item1.Position - selc.Item2.Position;

            selc.Item1.SetConnection("Door", -dir);
            selc.Item2.SetConnection("Door", dir);

        }

        
    }

    public override void Init(ref LBSLayer layer)
    {
        name = "Schema HillClimbing";

        this.layer = layer;

        var graph = layer.GetModule<LBSRoomGraph>();

        var schema = layer.GetModule<LBSSchema>();
        var adam = new OptimizableSchema(schema);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(100); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluación
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(graph), 0.4f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(graph), 0.15f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(), 0.35f),
            new System.Tuple<IEvaluator, float> (new RoomCutEvaluator(graph), 1f),
            // new System.Tuple<IEvaluator, float> (new StretchEvaluator(), 0.1f),
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
            var walls = vWalls.Concat(hWalls).ToList();

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
        return new SchemaHCAgent(layer, id);
    }
}
