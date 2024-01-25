using System.Linq;
using System;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

public class WeightedEvaluator : IEvaluator
{
    Tuple<IEvaluator, float>[] evaluators;

    public WeightedEvaluator(params Tuple<IEvaluator, float>[] evaluators)
    {
        this.evaluators = evaluators;
    }

    public VisualElement CIGUI()
    {
        throw new NotImplementedException();
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var r = UnityEngine.Random.Range(0f, 1f);

        /*
        var clock = new Stopwatch();
        clock.Start();
        var fitness = SerieEVA(evaluable);
        clock.Stop();
        UnityEngine.Debug.Log("Fitness serie: ("+r+") " + clock.ElapsedMilliseconds / 1000f + "s.");
        */

        var clock = new Stopwatch();
        clock.Restart();
        var fitness2 = ParallelEVA(evaluable);
        clock.Stop();
        //UnityEngine.Debug.Log("Fitness paralelo: ("+r+") " + clock.ElapsedMilliseconds / 1000f + "s.");

        return fitness2;
    }

    private float SerieEVA(IOptimizable evaluable)
    {
        float totalWeight = 0;
        float fitness = 0; 
        
        foreach (var t in evaluators)
        {
            totalWeight += t.Item2;
            fitness += t.Item1.Evaluate(evaluable) * t.Item2;
        }
        return fitness / totalWeight;
    }

    private float ParallelEVA(IOptimizable evaluable)
    {

        float fitness = 0;
        float totalWeight = evaluators.ToList().Sum(e => e.Item2);

        // Crear y ejecutar tareas en paralelo
        Task[] tasks = new Task[evaluators.Count()];
        float[] results = new float[evaluators.Count()];

        Parallel.For(0, evaluators.Count(), i =>
        {
            var eva = evaluators[i];

            results[i] = eva.Item1.Evaluate(evaluable) * eva.Item2;
        });

        fitness = results.Sum();

        return fitness / totalWeight;
    }


    public string GetName()
    {
        throw new NotImplementedException();
    }
}