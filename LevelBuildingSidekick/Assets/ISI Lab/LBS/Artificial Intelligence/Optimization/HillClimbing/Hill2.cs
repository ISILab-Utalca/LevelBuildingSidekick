using System.Collections.Generic;
using UnityEngine;
using System;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using Commons.Optimization;
using LBS;
using System.Linq;
using Random = UnityEngine.Random;
using LBS.Components.TileMap;
using UnityEditor;
using LBS.Components.Graph;
using System.Diagnostics;
using GeneticSharp.Domain.Selections;
using System.Linq;
using GeneticSharp.Domain.Populations;

namespace Utility
{
    //Todo esto esta siendo usado en el panel AITest, cambiar nombre o reemplazar lo de la clase HillClimbing por esta.
    //Division de los metodos y estados basados en el Genetic.
    public class Hill2 : BaseOptimizer
    {
        private System.Random random;

        public Hill2(IOptimizable adam, IPopulation population, IEvaluator evaluator, ISelection selection, ITermination termination) : base( adam, population, evaluator, selection, termination)
        {
        }

        public override void RunOnce()
        {
            var parents = Selection.SelectEvaluables(1, Population.CurrentGeneration);

            BestCandidate = Population.CurrentGeneration.BestCandidate;

            var offsprings = GetNeighbors(BestCandidate);

            Population.CreateNewGeneration(offsprings);

            offsprings.ForEach(c =>
            {
                Evaluator.Evaluate(c);
            });

        }

        public override List<IOptimizable> GetNeighbors(IOptimizable Adam)
        {
            var tileMap = Adam as AreaTileMap<TiledArea>;
            var neightbours = new List<IOptimizable>();
            var maxSize = LBSController.CurrentLevel.data.MaxSize;

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
}