using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using System.Linq;
using LBS;
using LBS.Representation.TileMap;

public class TagProportionByRoom : IRangedEvaluator
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

        var indexG1 = new List<int>();
        var indexG2 = new List<int>();

        if (pressetsG1.Count() == 0 || pressetsG2.Count() == 0)
        {
            if (pressetsG1.Count() == 0 && pressetsG2.Count() == 0)
            {
                return MaxValue;
            }

            return MinValue;
        }

        foreach (var pr in pressetsG1)
        {
            indexG1.Add(stmc.stamps.FindIndex(s => s.Label == pr));
        }

        foreach (var pr in pressetsG2)
        {
            indexG2.Add(stmc.stamps.FindIndex(s => s.Label == pr));
        }

        var rooms = (StampTileMapChromosome.TileMap.GetData() as LBSSchemaData).GetRooms();

        var fitness = 0;

        foreach (var r in rooms)
        {

            int counterG1 = 0;
            int counterG2 = 0;

            foreach (var t in r.Tiles)
            {
                int val = stmc.GetGene<int>(stmc.ToIndex(t.GetPosition()));
                if (val == -1) continue;
                if (indexG1.Contains(val))
                {
                    counterG1++;
                }
                else if(indexG2.Contains(stmc.GetGene<int>(stmc.ToIndex(t.GetPosition()))))
                {
                    counterG2++;
                }
            }
            var c = counterG1 / counterG2;
            c = c > 1 ? 1 / c : c;
            fitness += c;
        }

        return fitness/rooms.Count;
    }

    public string GetName()
    {
        return "Tag Proportion";
    }
}
