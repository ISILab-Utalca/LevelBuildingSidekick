using Commons.Optimization;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
// using static Unity.Collections.AllocatorManager;
/*
public class Genetic2 : BaseOptimizerMetahuristic<IEvaluable>
{

    private bool m_stopRequested;

    public Genetic2() : base()
    {
        Reinsertion = new ElitistReinsertion();
        CrossoverProbability = DefaultCrossoverProbability;
        MutationProbability = DefaultMutationProbability;
        TimeEvolving = TimeSpan.Zero;
        TaskExecutor = new LinearTaskExecutor();
        OperatorsStrategy = new DefaultOperatorsStrategy();

        Selection = new RankSelection();
        Crossover = new UniformCrossover();
        Mutation = new UniformMutation();
        Population = new Population();
        Termination = new GenerationNumberTermination();
    }

    #region Constants
    /// <summary>
    /// The default crossover probability.
    /// </summary>
    public const float DefaultCrossoverProbability = 0.75f;

    /// <summary>
    /// The default mutation probability.
    /// </summary>
    public const float DefaultMutationProbability = 0.25f;

    public UniformCrossover Crossover { get; private set; }
    public UniformMutation Mutation { get; private set; }
    public ElitistReinsertion Reinsertion { get; private set; }
    public LinearTaskExecutor TaskExecutor { get; private set; }
    public DefaultOperatorsStrategy OperatorsStrategy { get; private set; }
    public RankSelection Selection { get; private set; }

    private float CrossoverProbability;
    private float MutationProbability;
    #endregion




    public override string GetName()
    {
        throw new NotImplementedException();
    }

    private bool EvolveOneGeneration()
    {
        var parents = SelectParents();
        var offspring = Cross(parents);
        Mutate(offspring);
        var newGenerationChromosomes = Reinsert(offspring, parents);
        Population.CreateNewGeneration(newGenerationChromosomes);
        return EndCurrentGeneration();
    }

    private IList<IEvaluable> Cross(IList<IEvaluable> parents)
    {
        return OperatorsStrategy.Cross(Population, Crossover, CrossoverProbability, parents);
    }

    private IList<IEvaluable> SelectParents()
    {
        return Selection.SelectEvaluables(Population.MinSize, Population.CurrentGeneration);
    }

    private void Mutate(IList<IEvaluable> chromosomes)
    {
        OperatorsStrategy.Mutate(Mutation, MutationProbability, chromosomes);
    }

    private IList<IEvaluable> Reinsert(IList<IEvaluable> offspring, IList<IEvaluable> parents)
    {
        return Reinsertion.SelectChromosomes(Population, offspring, parents);
    }

    private bool EndCurrentGeneration()
    {
        EvaluateFitness();
        Population.EndCurrentGeneration();

        OnGenerationRan?.Invoke();
        
        if (Termination.HasReached(this))
        {
            State = Op_State.TerminationReached;
            OnTerminationReached?.Invoke();
            return true;
        }

        if (m_stopRequested)
        {
            TaskExecutor.Stop();
            State = Op_State.Stopped;
        }
        

        return false;
    }
    private void EvaluateFitness()
    {
        try
        {
            var chromosomesWithoutFitness = Population.CurrentGeneration.Evaluables.Where(c => !c.Fitness.HasValue).ToList();

            for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
            {
                var c = chromosomesWithoutFitness[i];

                TaskExecutor.Add(() =>
                {
                    RunEvaluateFitness(c);
                });
            }

            if (!TaskExecutor.Start())
            {
                throw new TimeoutException("The fitness evaluation reached the {0} timeout.".With(TaskExecutor.Timeout));
            }
        }
        finally
        {
            TaskExecutor.Stop();
            TaskExecutor.Clear();
        }

        Population.CurrentGeneration.Evaluables = Population.CurrentGeneration.Evaluables.OrderByDescending(c => c.Fitness).ToList();
    }
    private void RunEvaluateFitness(object chromosome)
    {
        var c = chromosome as IChromosome;

        try
        {
            c.Fitness = Evaluator.Evaluate(c);
        }
        catch (Exception ex)
        {
            throw new FitnessException(Evaluator, "Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message), ex);
        }
    }


    public override IEvaluable RunOnce(IEvaluable evaluable, IEvaluator evaluator, ITermination termination)
    {
        throw new NotImplementedException();
    }

    public override List<IEvaluable> GetNeighbors(IEvaluable Adam)
    {
        throw new NotImplementedException();
    }


    
}



*/