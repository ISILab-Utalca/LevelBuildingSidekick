using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using System.Linq;
using LBS;

public class TagProportion : IRangedEvaluator
{

    public string tag1;
    public string tag2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;


    public VisualElement CIGUI()
    {
        var content = new VisualElement();
        var tags = Utility.DirectoryTools.GetScriptable<LBSTagLists>("Brush tags").Alls;

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
        tagDD1.RegisterCallback<ChangeEvent<string>>(e => tag1 = e.newValue);

        DropdownField tagDD2 = new DropdownField("Tag 2: ");
        tagDD2.choices = tags;
        tagDD2.index = index2;
        tagDD2.RegisterCallback<ChangeEvent<string>>(e => tag2 = e.newValue);

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

        if(pressetsG1.Count() == 0 || pressetsG2.Count() == 0)
        {
            if(pressetsG1.Count() == 0 && pressetsG2.Count() == 0)
            {
                return MaxValue;
            }

            return MinValue;
        }

        int counterG1 = 0;
        int counterG2 = 0;

        foreach (var id in data)
        {
            if(pressetsG1.Contains(stmc.stamps[id].Label))
            {
                counterG1++;
            }
            if (pressetsG2.Contains(stmc.stamps[id].Label))
            {
                counterG2 ++;
            }
        }

        var p = counterG1 / counterG2;

        return p > 1 ? 1 / p : p;
    }

    public string GetName()
    {
        return "Tag Proportion";
    }
}
