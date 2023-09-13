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
using GeneticSharp.Domain.Randomizations;

namespace ISILab.AI.Optimization
{
    public class StochasticHillClimbing : BaseOptimizer
    {
        Func<IOptimizable, List<IOptimizable>> GetNeighbors;
        public double Nlog = 0;
        public double NNlog = 0;
        public double Elog = 0;

        public StochasticHillClimbing(IPopulation population, IEvaluator evaluator, ISelection selection, Func<IOptimizable, List<IOptimizable>> getNeighbors, ITermination termination) : base(population, evaluator, selection, termination)
        {
            GetNeighbors = getNeighbors;
        }

        public override void EvaluateFitness(IList<IOptimizable> optimizables)
        {
            foreach (var o in optimizables)
            {
                o.Fitness = Evaluator.Evaluate(o);
            }
        }

        public override void RunOnce()
        {
            var clock = new Stopwatch();

            var last = Population.Generations.Last();
            var selection = last.Evaluables;

            if (selection.Count == 0)
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
            NNlog = offsprings.Count();

            var candidates = offsprings.OrderBy(n => RandomizationProvider.Current.GetDouble());

            IOptimizable next = null;
            clock.Restart();
            foreach (var os in candidates)
            {
                os.Fitness = Evaluator.Evaluate(os);
                if (os.Fitness > best.Fitness)
                {
                    next = os;
                    break;
                }
            }
            clock.Stop();
            Elog = clock.ElapsedMilliseconds;

            if (next == null)
            {
                Stop();
            }
            else
            {
                Population.CreateNewGeneration(new List<IOptimizable>() { next });
                Population.EndCurrentGeneration();
            }

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
