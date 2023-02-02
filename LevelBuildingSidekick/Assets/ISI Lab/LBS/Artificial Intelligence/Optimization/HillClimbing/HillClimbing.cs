using System.Collections.Generic;
using UnityEngine;
using System;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using Commons.Optimization;
using LBS;
using System.Linq;
using LBS.Components.TileMap;
using UnityEditor;
using LBS.Components.Graph;
using System.Diagnostics;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Populations;

namespace LBS.AI
{
    //Todo esto esta siendo usado en el panel AITest, cambiar nombre o reemplazar lo de la clase HillClimbing por esta.
    //Division de los metodos y estados basados en el Genetic.
    public class HillClimbing : BaseOptimizer
    {
        Func<IOptimizable, List<IOptimizable>> GetNeighbors;

        public HillClimbing(IPopulation population, IEvaluator evaluator, ISelection selection, Func<IOptimizable, List<IOptimizable>> getNeighbors,  ITermination termination) : base( population, evaluator, selection, termination)
        {
            GetNeighbors = getNeighbors;
        }

        public override void RunOnce()
        {
            var parents = Selection.SelectEvaluables(1, Population.CurrentGeneration);

            BestCandidate = Population.CurrentGeneration.BestCandidate;

            var offsprings = GetNeighbors?.Invoke(BestCandidate);

            Population.CreateNewGeneration(offsprings);

            offsprings.ForEach(c =>
            {
                Evaluator.Evaluate(c);
            });

        }

        
    }
}