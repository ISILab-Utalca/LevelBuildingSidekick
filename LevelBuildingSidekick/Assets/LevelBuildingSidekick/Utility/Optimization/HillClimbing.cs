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
        public static T Run<T>(T root, System.Func<bool> endCondition, System.Func<T, List<T>> GetNeighbors, System.Func<T, float> Evaluate)
        {
            score = 0;
            prevScore = score;
            T best = root;
            while (endCondition?.Invoke() == false)
            {
                prevScore = score;
                score = Evaluate(best);
                List<T> candidates = GetNeighbors(best);
                foreach (T candidate in candidates)
                {
                    float newScore = Evaluate(candidate);
                    if (newScore > score)
                    {
                        best = candidate;
                        score = newScore;
                        nonSignificantEpochs = 0;
                    }
                }
                if(score == prevScore)
                {
                    nonSignificantEpochs++;
                }

            }
            return best;
        }
    }
}

