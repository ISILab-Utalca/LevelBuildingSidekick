using Commons.Optimization.Evaluator;
using GeneticSharp.Domain.Chromosomes;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[CustomVisualElement(typeof(Resemblance))]
public class ResemblanceVE : EvaluatorVE
{
    DropdownField dropdown;

    public ResemblanceVE(IEvaluator evaluator) : base(evaluator)
    {
        dropdown = new DropdownField();
        dropdown.label = "Reference";
        dropdown.RegisterValueChangedCallback(SetChromosome);
    }

    private List<string> GetChromosomables()
    {
        List<string> choices = new List<string>();

        foreach(var m in layer.Modules)
        {
            var ves = Utility.Reflection.GetClassesWith<ChromosomeFromModuleAttribute>().Where(t => t.Item2.Any(v => v.type == m.GetType()));
            if(ves.Count() != 0)
            {
                choices.Add(m.Key);
            }
        }

        return choices;
    }

    private void SetChromosome(ChangeEvent<string> e)
    {
        var module = layer.GetModule<LBSModule>(e.newValue);

        if(module == null)
        {
            throw new Exception("[ISI Lab] Module not found");
        }

        var type = module.GetType();

        var target = Reflection.GetClassesWith<ChromosomeFromModuleAttribute>().Where(t => t.Item2.Any(v => v.type == type)).First().Item1;

        var chrom = Activator.CreateInstance(target, new object[] { module }) as IChromosome;

        (evaluator as Resemblance).reference = chrom;
    }

    public override void SetLayer(LBSLayer layer)
    {
        base.SetLayer(layer);
        if (dropdown != null)
            dropdown.choices = GetChromosomables();
    }
}
