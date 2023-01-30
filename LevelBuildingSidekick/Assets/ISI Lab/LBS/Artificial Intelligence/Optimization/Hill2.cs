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

namespace Utility
{
    public class Hill2<u> : BaseOptimizerMetahuristic<IEvaluable>
    {
        u heuristic;
        IEvaluator Evaluable;
        public Hill2( u heu ) : base(){

            heuristic = heu;
            Evaluable = new WeightedEvaluator();
        }
        static float prevScore = 0;

        
        public static float PrevScore
        {
            get
            {
                return prevScore;
            }
        }

        static float score = 0;
        public static float Score
        {
            get
            {
                return score;
            }
        }

        static int nonSignificantEpochs = 0;
        public static int NonSignificantEpochs
        {
            get
            {
                return nonSignificantEpochs;
            }
        }


        public override IEvaluable RunOnce(IEvaluable root, IEvaluator Evaluate, ITermination Terminator)
        {
            int iterations = 0;

            //System.Random random = (seed != null) ? new System.Random((int)seed) : new System.Random();
            var random = new System.Random();
            score = prevScore = 0;
            nonSignificantEpochs = 0;

            var best = root;
            while (!TerminatioReached()) //(!endCondition?.Invoke())
            {
                iterations++;

                prevScore = score;
                score = Evaluate.EvaluateH(best, heuristic);
                List<IEvaluable> candidates = GetNeighbors(root);
                List<IEvaluable> betters = new List<IEvaluable>();

                float higherScore = score;
                for (int i = 0; i < candidates.Count; i++)
                {
                    float newScore = Evaluate.EvaluateH(candidates[i], heuristic);
                    if (newScore > higherScore)
                    {
                        betters.Clear();
                        betters.Add(candidates[i]);
                    }
                    else if (newScore == higherScore)
                    {
                        betters.Add(candidates[i]);
                    }
                    higherScore = higherScore < newScore ? newScore : higherScore;

                }
                if (higherScore <= score)
                {
                    nonSignificantEpochs++;
                }
                else
                {
                    nonSignificantEpochs = 0;
                }

                if (betters.Count == 0)
                {
                    return best;
                }

                best = betters[random.Next(0, betters.Count - 1)];

                if (best is LBSSchemaData)
                {
                    //(best as LBSSchemaData).Print(); // debug temporal quitar luego
                    //Debug.Log(higherScore);
                }
                /*
                if (debug)
                {
                    var msg = "";
                    msg = "<b> Iteration '" + iterations + "'</b>\n";
                    msg = "candidates: " + candidates.Count;
                    msg = "betters: " + betters.Count;
                    msg = "better score: " + score;
                    Debug.Log(msg);
                }*/
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
                var room = tileMap.GetRoom(i);
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

       
        
    }
}