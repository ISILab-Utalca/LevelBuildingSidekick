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

public class SchemaHCAgent : LBSAIAgent
{
    HillClimbing hillClimbing;

    public override void Execute()
    {
        hillClimbing.Run();
    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(LBSLayer layer)
    {
        var selection = new EliteSelection();
        var termination = new OrTermination(new ITermination[] { new FitnessStagnationTermination()}); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluación
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(), 0.5f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(), 0.35f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(), 0.15f),

        }) ;
        var population = new Population(); // agregar parametros

        hillClimbing = new HillClimbing(null, population, evaluator, selection, GetNeighbors, termination); // asignar Adam

    }

    public List<IOptimizable> GetNeighbors(IOptimizable Adam)
    {
        var tileMap = layer.GetModule<AreaTileMap<TiledArea>>(); // Adam as algun tipo de IOptimizable que usa schema
        var neightbours = new List<IOptimizable>();

        for (int i = 0; i < tileMap.RoomCount; i++)
        {
            var room = tileMap.GetArea(i);
            var vWalls = room.GetVerticalWalls();
            var hWalls = room.GetHorizontalWalls();
            var walls = vWalls.Concat(hWalls);

            foreach (var wall in walls)
            {
                var neighbor = tileMap.Clone() as AreaTileMap<TiledArea>;

                var tiles = new TiledArea();
                wall.Tiles.ForEach(t => tiles.AddTile(new ConnectedTile(t + wall.Dir, room.ID, 4)));
                neighbor.AddArea(tiles);

                neightbours.Add(neighbor as IOptimizable);
            }

            foreach (var wall in walls)
            {
                var neighbor = tileMap.Clone() as LBSSchema;
                neighbor.Clear();
                neightbours.Add(neighbor as IOptimizable);
            }

            // Change the room size
            var newSize = new Vector2(Random.Range(1, room.Size.x), Random.Range(1, room.Size.y));

            for (int x = 0; x < newSize.x; x++)
            {
                var newTiles = new TiledArea();
                var neighbor = tileMap.Clone() as LBSSchema;

                for (int y = 0; y < newSize.y; y++)
                {
                    newTiles.AddTile(new ConnectedTile(new Vector2Int((int)room.Centroid.x + x, (int)room.Centroid.y + y), room.ID, 4));
                }

                neighbor.AddArea(newTiles);

                neightbours.Add(neighbor as IOptimizable);
            }
        }

        return neightbours;
    }
}
