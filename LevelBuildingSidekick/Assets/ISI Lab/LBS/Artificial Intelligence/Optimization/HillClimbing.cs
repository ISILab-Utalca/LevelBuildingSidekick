using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utility
{
    public class HillClimbing
    {
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


        /// <summary>
        /// Runs the optimization algorithm with the specified root node, end condition,function for getting neighbors,
        /// and function for evaluating a node.
        /// </summary>
        /// <typeparam name="T">The type of the root node and the neighbors.</typeparam>
        /// <param name="root">The root node to start the optimization from.</param>
        /// <param name="endCondition">A function that returns a boolean indicating whether the optimization should end.
        /// The optimization ends when this function returns true.</param>
        /// <param name="GetNeighbors">A function that takes in a node and returns a list of its neighbors.</param>
        /// <param name="Evaluate">A function that takes in a node and returns a float indicating the node's value for the optimization.</param>
        /// <returns>The optimized node of type T.</returns>
        public static T Run<T>(T root, System.Func<bool> endCondition, System.Func<T, List<T>> GetNeighbors, System.Func<T, float> Evaluate)
        {
            var r = new System.Random();
            score = 0;
            nonSignificantEpochs = 0;
            prevScore = score;
            T best = root;
            while (endCondition?.Invoke() == false)
            {
                prevScore = score;
                score = Evaluate(best);
                List<T> candidates = GetNeighbors(best);
                List<T> betters = new List<T>();
                Debug.Log("Before: " + candidates.Count);
                float higherScore = score;
                for(int i = 0; i < candidates.Count; i++)
                {
                    float newScore = Evaluate(candidates[i]);
                    if (newScore > higherScore)
                    {
                        betters.Clear();
                        betters.Add(candidates[i]);
                    }
                    else if(newScore == higherScore)
                    {
                        betters.Add(candidates[i]);
                    }
                    higherScore = higherScore < newScore ? newScore : higherScore;
                    //Debug.Log("New Score: " + newScore);

                }
                if(higherScore <= score)
                {
                    nonSignificantEpochs++;
                }
                else
                {
                    nonSignificantEpochs = 0;
                }
                if(betters.Count == 0)
                {
                    return best;
                }
                Debug.Log("After: " + betters.Count);
                best = betters[r.Next(0, betters.Count - 1)];
                Debug.Log("Score: " + score);
            }
            return best;
        }


        /// <summary>
        /// Performs an optimization process using the provided root node, heuristic, and evaluation functions.
        /// </summary>
        /// <typeparam name="T">The type of the nodes in the optimization process.</typeparam>
        /// <typeparam name="U">The type of the heuristic value.</typeparam>
        /// <param name="root">The root node to start the optimization process from.</param>
        /// <param name="heuristic">The heuristic value to use in the evaluation function.</param>
        /// <param name="endCondition">A function that returns a boolean indicating whether the optimization process should end.</param>
        /// <param name="GetNeighbors">A function that returns a list of neighboring nodes to the provided node.</param>
        /// <param name="Evaluate">A function that returns a score for the provided node and heuristic value.</param>
        /// <param name="seed">An optional seed value for the random number generator.</param>
        /// <param name="debug">A boolean indicating whether debug information should be logged during the optimization process.</param>
        /// <returns>The best node found during the optimization process.</returns>
        public static T Run<T, U>(T root, U heuristic, System.Func<bool> endCondition, System.Func<T, List<T>> GetNeighbors, System.Func<T, U, float> Evaluate,int? seed = null, bool debug = false)
        {
            int iterations = 0;

            System.Random random = (seed != null)? new System.Random((int)seed) : new System.Random();

            score = prevScore = 0;
            nonSignificantEpochs = 0;

            T best = root;
            while (endCondition?.Invoke() == false) //(!endCondition?.Invoke())
            {
                iterations++;

                prevScore = score;
                score = Evaluate(best, heuristic);  
                List<T> candidates = GetNeighbors(best);
                List<T> betters = new List<T>();
                
                float higherScore = score;
                for (int i = 0; i < candidates.Count; i++)
                {
                    float newScore = Evaluate(candidates[i], heuristic);
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

                //if (best is LBSSchemaData)
                {
                    //(best as LBSSchemaData).Print(); // debug temporal quitar luego
                    //Debug.Log(higherScore);
                }

                if (debug)
                {
                    var msg = "";
                    msg = "<b> Iteration '" + iterations + "'</b>\n";
                    msg = "candidates: " + candidates.Count;
                    msg = "betters: " + betters.Count;
                    msg = "better score: " + score;
                    Debug.Log(msg);
                }
            }

            return best;
        }

    }
}

