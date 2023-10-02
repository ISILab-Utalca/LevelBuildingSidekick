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
    DropdownField referenceRectField;

    Rect rect;

    public ResemblanceVE(IEvaluator evaluator) : base(evaluator)
    {
        dropdown = new DropdownField();
        dropdown.label = "Reference";
        referenceRectField = new DropdownField();
        referenceRectField.label = "Bounds Reference";
        referenceRectField.RegisterValueChangedCallback(e =>
        {
            var mods = this.layer.Parent.Layers.SelectMany(l => l.Modules);
            rect = mods.ToList().Find(m => m.ID == e.newValue).GetBounds();
        });
        Add(dropdown);
        Add(referenceRectField);
    }

    private void GetChromosomables()
    {
        dropdown.choices = new List<string>();

        foreach(var m in layer.Modules)
        {
            /*var ves = Utility.Reflection.GetClassesWith<ChromosomeFromBehaviourAttribute>().Where(t => t.Item2.Any(v => v.type == m.GetType()));
            if(ves.Count() != 0)
            {
                dropdown.choices.Add(m.ID);
            }*/
        }

        dropdown.value = dropdown.choices[0];
    }


    private void GetTexturizables()
    {/*
        referenceRectField.choices = new List<string>();

        var mods = layer.Parent.Layers.SelectMany(l => l.Modules);

        foreach (var m in mods)
        {
            if (Reflection.GetClassesWith<ModuleTexturizerAttribute>().Any(t => t.Item2.Any(v => v.type == m.GetType())))
            {
                referenceRectField.choices.Add(m.ID);
            }
        }

        referenceRectField.value = referenceRectField.choices[0];
        rect = mods.ToList().Find(m => m.ID == referenceRectField.value).GetBounds();*/
    }

    private void SetChromosome(string mod)
    {
        var module = layer.GetModule<LBSModule>(mod);

        if(module == null)
        {
            throw new Exception("[ISI Lab] Module not found");
        }

        var type = module.GetType();

        /*var target = Reflection.GetClassesWith<ChromosomeFromBehaviourAttribute>().Where(t => t.Item2.Any(v => v.type == type)).First().Item1;

        var chrom = Activator.CreateInstance(target, new object[] { module, rect, null }) as ChromosomeBase;

        (evaluator as Resemblance).reference = chrom;*/


    }

    public override void SetLayer(LBSLayer layer)
    {
        base.SetLayer(layer);

        GetChromosomables();

        GetTexturizables();
    }

    public override void Init()
    {
        SetChromosome(dropdown.value);
    }
}
