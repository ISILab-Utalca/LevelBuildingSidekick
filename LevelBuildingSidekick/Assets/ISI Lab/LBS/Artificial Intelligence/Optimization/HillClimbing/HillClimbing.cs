using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Commons.Optimization;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Populations;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Terminations;
using LBS;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

        public float _nbrsTimer = 0;
        public float _fitTimer = 0;

        public HillClimbing(IPopulation population, IEvaluator evaluator, ISelection selection, Func<IOptimizable, List<IOptimizable>> getNeighbors,  ITermination termination) : base( population, evaluator, selection, termination)
        {
            GetNeighbors = getNeighbors;
        }

        public override void EvaluateFitness(IList<IOptimizable> optimizables, Action<float> onProgress = null, CancellationToken token = default)
        {
            for (var index = 0; index < optimizables.Count; index++)
            {
                var o = optimizables[index];
                o.Fitness = Evaluator.Evaluate(o);
                // Update progress
                onProgress?.Invoke(0.5f + 0.5f * (float)index / optimizables.Count);
            }
        }

        public override void RunOnce(Action<float> onProgress = null, CancellationToken token = default)
        {
            var clock = new Stopwatch();

            var last = Population.Generations.Last();
            
            clock.Restart();
            var selection = this.Selection.SelectEvaluables(1, last);

            if(selection.Count == 0)
            {
                Stop();
            }

            var best = selection.First();


            if (GetNeighbors == null)
                throw new NullReferenceException();

            clock.Restart();
         
            List<IOptimizable> offsprings = new List<IOptimizable>();
            var invocations = GetNeighbors.GetInvocationList();

            for (var index = 0; index < invocations.Length; index++)
            {
                var del = (Func<IOptimizable, List<IOptimizable>>)invocations[index];

                // Update progress
                onProgress?.Invoke( 0.5f *  (float)index / invocations.Length);

                // Invoke and flatten
                var result = del(best);
                if (result != null)  offsprings.AddRange(result);
            }
            
            // this is the old version
           // List<IOptimizable> offsprings = GetNeighbors.Invoke(best);
            
            clock.Stop();
            _nbrsTimer = clock.ElapsedMilliseconds / 1000f;

            Nlog = clock.ElapsedMilliseconds;
            NNlog= offsprings.Count;
            //var offsprings = GetNeighbors?.Invoke(BestCandidate); // poner exepcion por si neigthbor es null (!!!)

            if (offsprings.Count == 0)
            {
                Debug.LogError("No offspring!");
                return;
            }
            
            clock.Restart();
            EvaluateFitness(offsprings, onProgress, token);
            clock.Stop();
            _fitTimer = clock.ElapsedMilliseconds / 1000f;

            Elog = clock.ElapsedMilliseconds;

            //PrintClocks();

            Population.CreateNewGeneration(offsprings);
            Population.EndCurrentGeneration();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void PrintClocks()
        {
            UnityEngine.Debug.Log("Neighbors: " + _nbrsTimer + "s.");
            UnityEngine.Debug.Log("Fitness: " + _fitTimer + "s.");

        }

        public override void InitializeDefault()
        {
            throw new NotImplementedException();
        }
    }
}