using System.Collections.Generic;
using UnityEngine;
using System;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Terminations;
using Commons.Optimization;
using LBS;
using System.Linq;
using LBS.Components.TileMap;
using UnityEditor;
using LBS.Components.Graph;
using System.Diagnostics;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Populations;

namespace ISILab.AI.Optimization
{
    //Todo esto esta siendo usado en el panel AITest, cambiar nombre o reemplazar lo de la clase HillClimbing por esta.
    //Division de los metodos y estados basados en el Genetic.
    public class HillClimbing : BaseOptimizer
    {
        Func<IOptimizable, List<IOptimizable>> GetNeighbors;
        public double Nlog = 0;
        public double NNlog = 0;
        public double Elog = 0;

        public HillClimbing(IPopulation population, IEvaluator evaluator, ISelection selection, Func<IOptimizable, List<IOptimizable>> getNeighbors,  ITermination termination) : base( population, evaluator, selection, termination)
        {
            GetNeighbors = getNeighbors;
        }

        public override void EvaluateFitness(IList<IOptimizable> optimizables)
        {
            foreach(var o in optimizables)
            {
                o.Fitness = Evaluator.Evaluate(o);
            }
        }

        public override void RunOnce()
        {
            var clock = new Stopwatch();

            var last = Population.Generations.Last();
            var selection = this.Selection.SelectEvaluables(1, last);

            if(selection.Count == 0)
            {
                Stop();
            }

            var best = selection.First();

            if (GetNeighbors == null)
                throw new NullReferenceException();

            clock.Restart();
            var offsprings = GetNeighbors.Invoke(best);
            clock.Stop();
            Nlog = clock.ElapsedMilliseconds;
            NNlog= offsprings.Count;
            //var offsprings = GetNeighbors?.Invoke(BestCandidate); // poner exepcion por si neigthbor es null (!!!)

            clock.Restart();
            EvaluateFitness(offsprings);
            clock.Stop();
            Elog = clock.ElapsedMilliseconds;

            Population.CreateNewGeneration(offsprings);
            Population.EndCurrentGeneration();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}