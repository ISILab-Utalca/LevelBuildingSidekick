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

        public override void EvaluateFitness(IList<IOptimizable> optimizables)
        {
            //throw new NotImplementedException();
        }

        public override void RunOnce()
        {
            var last = Population.Generations.Last();
            var best = this.Selection.SelectEvaluables(1, last).First();

            if (GetNeighbors == null)
                throw new NullReferenceException();

            var offsprings = GetNeighbors.Invoke(best);
            //var offsprings = GetNeighbors?.Invoke(BestCandidate); // poner exepcion por si neigthbor es null (!!!)

            offsprings.ForEach(c =>
            {
                c.Fitness =  Evaluator.Evaluate(c);
            });

            Population.CreateNewGeneration(offsprings);


        }

        
    }
}