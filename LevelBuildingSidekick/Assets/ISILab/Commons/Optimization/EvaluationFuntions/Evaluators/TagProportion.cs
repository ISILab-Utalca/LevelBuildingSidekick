using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using System.Linq;
using LBS;
using System;

public class TagProportion : IRangedEvaluator
{

    public string tag1;
    public string tag2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

    public TagProportion()
    {
        var list = Utility.DirectoryTools.GetScriptable<LBSTags>("Brush tags").Alls;
        tag1 = list.First();
        tag2 = list.Last();
    }

    public VisualElement CIGUI()
    {
        var content = new VisualElement();
        var tags = Utility.DirectoryTools.GetScriptable<LBSTags>("Brush tags").Alls;

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        var index1 = tags.FindIndex(t => t == tag1);
        var index2 = tags.FindIndex(t => t == tag2);

        DropdownField tagDD1 = new DropdownField("Tag 1: ");
        tagDD1.choices = tags;
        tagDD1.index = index1;
        tagDD1.RegisterValueChangedCallback(e => tag1 = e.newValue);

        DropdownField tagDD2 = new DropdownField("Tag 2: ");
        tagDD2.choices = tags;
        tagDD2.index = index2;
        tagDD2.RegisterValueChangedCallback(e => tag2 = e.newValue);

        content.Add(v2);
        content.Add(tagDD1);
        content.Add(tagDD2);

        return content;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        if(!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        var data = stmc.GetGenes<int>();

        var pressets = Utility.DirectoryTools.GetScriptables<StampPresset>();

        var pressetsG1 = pressets.Where(p => p.Tags.Contains(tag1)).Select(p => p.Label);
        var pressetsG2 = pressets.Where(p => p.Tags.Contains(tag2)).Select(p => p.Label);

        if (pressetsG1.Count() == 0 || pressetsG2.Count() == 0)
        {
            return MinValue; // Temporal Fix, SHould be changed
            /*if (pressetsG1.Count() == pressetsG2.Count())
            {
                return MaxValue;
            }*/

        }

        var foundS1 = stmc.stamps.Any(s => pressetsG1.Contains(s.Label));
        var founsS2 = stmc.stamps.Any(s => pressetsG2.Contains(s.Label));

        if (!foundS1 || !founsS2)
        {
            return foundS1 != founsS2 ? MinValue : MaxValue;
        }

        float counterG1 = 0;
        float counterG2 = 0;

        foreach (var id in data)
        {
            if (id == -1)
                continue;
            if (pressetsG1.Contains(stmc.stamps[id].Label))
            {
                counterG1++;
            }
            if (pressetsG2.Contains(stmc.stamps[id].Label))
            {
                counterG2 ++;
            }
        }

        if ((counterG1 == 0 || counterG2 == 0))
        {
            return MinValue; // Temporal Fix, SHould be changed
            //return counterG1 != counterG2 ? MinValue : MaxValue;
        }

        var p = counterG1 / counterG2;

        return p > 1 ? 1 / p : p;
    }

    public string GetName()
    {
        return "Tag Proportion";
    }
}
