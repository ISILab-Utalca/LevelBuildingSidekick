using Optimization.Data;
using Optimization.Evaluators;
using Optimization.Neigbors;
using Optimization.Terminators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Optimization
{
    [Obsolete]
    public class SimulatedAnnealing
    {
        private float currentTemp = 1000;
        private float coolingRate = 0.1f;

        public Map Execute(Map init, float temp, IEvaluator evaluator, ITerminator terminator, IGetNeighbors getNeighbors)
        {
            // Init
            Map best = init;
            currentTemp = temp;

            // Algorithm
            var fitness = evaluator.Execute(best);
            while (currentTemp > 1)
            {
                var selected = getNeighbors.Execute(best)
                    .Select(n => (n.Item1 as Map, n.Item2))
                    .ToList(); // OPTIMIZE:.toList()!

                for (int i = 0; i < selected.Count; i++)
                {
                    if (terminator?.Execute() ?? false)
                    {
                        return best;
                    }

                    var (neig, move) = selected[i];
                    var newFitness = evaluator.Execute(neig);

                    var diff = newFitness - fitness;

                    if (diff > 0)
                    {
                        best = neig;
                        fitness = newFitness;
                    }
                    else
                    {
                        var prob = Mathf.Exp(-diff / currentTemp);
                        if (prob > UnityEngine.Random.Range(0.0f, 1.0f))
                        {
                            best = neig;
                            fitness = newFitness;
                            currentTemp *= 1 - coolingRate;
                        }
                    }
                }
            }

            return best;
        }
    }
}