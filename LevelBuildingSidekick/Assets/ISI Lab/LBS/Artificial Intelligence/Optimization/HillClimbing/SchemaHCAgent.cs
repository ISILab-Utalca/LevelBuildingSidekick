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

        Debug.Log("HillClimbing start!");
        hillClimbing.Run();
        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        var xx = new AreaToTileMap();
        xx.ParcheDiParche<TiledArea>(x);

        layer.SetModule<LBSSchema>(x , x.Key);


        Debug.Log("HillClimbing finish!");
    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(ref LBSLayer layer)
    {
        name = "Schema HillClimbing";

        this.layer = layer;

        var graph = layer.GetModule<LBSRoomGraph>();

        var schema = layer.GetModule<LBSSchema>();
        var adam = new OptimizableSchema(schema);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(15); // agregar termination de maximo local
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
        var tileMap = (Adam as OptimizableSchema).Schema; // Adam as algun tipo de IOptimizable que usa schema
        var neighbours = new List<IOptimizable>();

        for (int i = 0; i < tileMap.RoomCount; i++)
        {
            var room = tileMap.GetArea(i);
            var vWalls = room.GetVerticalWalls();
            var hWalls = room.GetHorizontalWalls();
            var walls = vWalls.Concat(hWalls);

            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                var tiles = neighbour.GetArea(i);
                wall.Tiles.ForEach(t => tiles.AddTile(new ConnectedTile(t + wall.Dir, room.ID, 4)));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            foreach (var wall in walls)
            {
                var neighbour = tileMap.Clone() as LBSSchema;

                var tiles = neighbour.GetArea(i);
                wall.Tiles.ForEach(t => tiles.RemoveAt(t));

                neighbours.Add(new OptimizableSchema(neighbour));
            }

            /*
            // Change the room size
            var newSize = new Vector2(Random.Range(1, room.Size.x), Random.Range(1, room.Size.y));

            for (int x = 0; x < newSize.x; x++)
            {
                var neighbour = tileMap.Clone() as LBSSchema;ss
                var newTiles = neighbour.GetArea(i);

                for (int y = 0; y < newSize.y; y++)
                {
                    newTiles.AddTile(new ConnectedTile(new Vector2Int((int)room.Centroid.x + x, (int)room.Centroid.y + y), room.ID, 4));
                }

                neighbor.AddArea(newTiles);

                neighbours.Add(neighbor as IOptimizable);
            }*/
        }

        return neighbours;
    }
}
