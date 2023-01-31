using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LBS.Representation.TileMap;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using Commons.Optimization;
using LBS;
using System.Linq;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using LBS.Components.TileMap;
using UnityEditor;
using LBS.Components.Graph;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static Unity.Collections.AllocatorManager;
using System.Diagnostics;
using GeneticSharp.Domain.Selections;

namespace Utility
{
    public class Hill2<U> : BaseOptimizerMetahuristic<IEvaluable>
    {
        U heuristic;
        static double prevScore { get; set; }
        static float score  { get; set; }
        static int nonSignificantEpochs { get; set; }
        private System.Random random;
        private int iterations;
        private float higherScore;
        IEvaluable best;
        ISelection Selection;
        List<IEvaluable> candidates;

        public Hill2( U heu ) : base(){

            heuristic = heu;
            candidates = GetNeighbors(Adam);
            Selection =  new HCSelector<U>(candidates, heuristic);
            Termination = new FitnessStagnationTermination();
            //Evaluator = new WeightedEvaluator();
        }

        public override void Start()
        {
            OnStarted?.Invoke();
            lock (m_lock)
            {
                //System.Random random = (seed != null) ? new System.Random((int)seed) : new System.Random();
                iterations = GenerationsNumber; // <---(?)
                random = new System.Random();
                prevScore = BestCandidate.Fitness.Value;
                nonSignificantEpochs = 0;
                best = Adam;
                stopRequested = false;
                pauseRequested = false;
                State = Op_State.Started;
                clock = Stopwatch.StartNew();
            }

            Run();
        }


        public override IEvaluable Run()
        {
            while (!TerminatioReached()) //(!endCondition?.Invoke())
            {
                iterations++;

                prevScore = BestCandidate.Fitness.Value;
                BestCandidate.Fitness = Evaluator.EvaluateH(best, heuristic);
                
                List<IEvaluable> betters = new List<IEvaluable>();

                //Inicio Selection
                betters = Selection.GetBetters(Evaluator, BestCandidate.Fitness);
                //Fin Selection

                //Inicio Termination
                
                //Fin Termination

                if (betters.Count == 0)
                {
                    return best;
                }

                best = betters[random.Next(0, betters.Count - 1)];
                
            }

            return best;
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override List<IEvaluable> GetNeighbors(IEvaluable Adam)
        {
            var tileMap = Adam as LBSSchema;
            var neightbours = new List<IEvaluable>();
            var maxSize = LBSController.CurrentLevel.data.MaxSize;

            for (int i = 0; i < tileMap.RoomCount; i++)
            {
                var room = tileMap.GetArea(i);
                var vWalls = room.GetVerticalWalls();
                var hWalls = room.GetHorizontalWalls();
                var walls = vWalls.Concat(hWalls);

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSSchema;

                    var tiles = new TiledArea<ConnectedTile>();
                    wall.Tiles.ForEach(t => tiles.AddTile(new ConnectedTile(t + wall.Dir, room.ID, 4)));
                    neighbor.AddArea(tiles);

                    neightbours.Add(neighbor as IEvaluable);
                }

                foreach (var wall in walls)
                {
                    var neighbor = tileMap.Clone() as LBSSchema;
                    neighbor.Clear();
                    neightbours.Add(neighbor as IEvaluable);
                }

                //Move rooms
                // Change the room size
                var newSize = new Vector2(Random.Range(1, room.Size.x), Random.Range(1, room.Size.y));

                for (int x = 0; x < newSize.x; x++)
                {
                    var newTiles = new TiledArea<ConnectedTile>();
                    var neighbor = tileMap.Clone() as LBSSchema;

                    for (int y = 0; y < newSize.y; y++)
                    {
                        newTiles.AddTile(new ConnectedTile(new Vector2Int((int)room.Centroid.x + x, (int)room.Centroid.y + y), room.ID, 4));
                    }

                    neighbor.AddArea(newTiles);

                    neightbours.Add(neighbor as IEvaluable);
                }
            }

            return neightbours;
        }

        public override IEvaluable RunOnce()
        {
            throw new NotImplementedException();
        }
    }
}