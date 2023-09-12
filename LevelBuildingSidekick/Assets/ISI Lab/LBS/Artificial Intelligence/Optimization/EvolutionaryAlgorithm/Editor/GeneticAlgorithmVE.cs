using Commons.Optimization.Evaluator;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using ISILab.AI.Optimization;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Terminations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("GeneticAlgorithm", typeof(GeneticAlgorithm))]
public class GeneticAlgorithmVE : LBSCustomEditor
{
    DynamicFoldout evaluator;
    DynamicFoldout selection;
    DynamicFoldout termination;

    DynamicFoldout mutation;
    FloatField mutationProbability;

    DynamicFoldout crossover;
    FloatField crossoverProbability;




    public GeneticAlgorithmVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }


    public override void SetInfo(object target)
    {
        this.target = target;
        var genetic = target as GeneticAlgorithm;
        if (genetic.Evaluator != null)
        {
            evaluator.SetInfo(genetic.Evaluator);
        }
        if (genetic.Selection != null)
        {
            selection.SetInfo(genetic.Selection);
        }
        if (genetic.Termination != null)
        {
            termination.SetInfo(genetic.Termination);
        }
        if (genetic.Mutation != null)
        {
            mutation.SetInfo(genetic.Mutation);
        }
        if (genetic.Crossover != null)
        {
            crossover.SetInfo(genetic.Crossover);
        }

        mutationProbability.value = genetic.MutationProbability;
        crossoverProbability.value = genetic.CrossoverProbability;
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var genetic = target as GeneticAlgorithm;

        #region Evaluator
        evaluator = new DynamicFoldout(typeof(IEvaluator));
        evaluator.Label = "Evaluator";
        if (genetic != null && genetic.Evaluator != null)
        {
            evaluator.Data = genetic.Evaluator;
        }

        evaluator.OnChoiceSelection += () => { genetic.Evaluator = evaluator.Data as IEvaluator; };
        #endregion

        #region Selection
        selection = new DynamicFoldout(typeof(ISelection));
        selection.Label = "Selection";
        if (genetic != null && genetic.Selection != null)
        {
            selection.Data = genetic.Selection;
        }

        selection.OnChoiceSelection += () => { genetic.Selection = selection.Data as ISelection; };
        #endregion

        #region Termination
        termination = new DynamicFoldout(typeof(ITermination));
        termination.Label = "Termination";
        if (genetic != null && genetic.Termination != null)
        {
            termination.Data = genetic.Termination;
        }

        termination.OnChoiceSelection += () => { genetic.Termination = termination.Data as ITermination; };
        #endregion

        #region Crossover
        crossover = new DynamicFoldout(typeof(ICrossover));
        crossover.Label = "Crossover";
        if (genetic != null && genetic.Crossover != null)
        {
            crossover.Data = genetic.Crossover;
        }

        crossover.OnChoiceSelection += () => { genetic.Crossover = crossover.Data as ICrossover; };
        #endregion

        #region Mutation
        mutation = new DynamicFoldout(typeof(IMutation));
        mutation.Label = "Mutation";
        if (genetic != null && genetic.Mutation != null)
        {
            mutation.Data = genetic.Mutation;
        }

        mutation.OnChoiceSelection += () => { genetic.Mutation = mutation.Data as IMutation; };
        #endregion

        crossoverProbability = new FloatField();
        crossoverProbability.label = "Crossover Probability";
        crossoverProbability.RegisterValueChangedCallback(e => { genetic.CrossoverProbability = e.newValue; });

        mutationProbability = new FloatField();
        mutationProbability.label = "Mutation Probability";
        mutationProbability.RegisterValueChangedCallback(e => { genetic.MutationProbability = e.newValue; });


        ve.Add(evaluator);
        ve.Add(selection);
        ve.Add(termination);
        ve.Add(crossover);
        ve.Add(mutation);
        ve.Add(crossoverProbability);
        ve.Add(mutationProbability);

        return ve;
    }
}
