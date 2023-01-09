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

    /// <summary>
    /// Initializes a new instance of <see cref="TagProportionByRoom"/> class.
    /// </summary>
    public TagProportionByRoom()
    {
        var list = Utility.DirectoryTools.GetScriptable<LBSTags>("Brush tags").Alls;
        tag1 = list.First();
        tag2 = list.Last();
    }

    /// <summary>
    /// Creates the GUI for the evaluator.
    /// </summary>
    /// <returns> A <see cref="VisualElement"/> containing the GUI. </returns>
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

    /// <summary>
    /// Compare the presence of one tag with another in each of the rooms.
    /// </summary>
    /// <param name="evaluable"></param>
    /// <returns> A float value representing the evaluation result. </returns>
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
            return MinValue;// Temporal Fix, Should be changed
            /*if (pressetsG1.Count() == 0 && pressetsG2.Count() == 0)
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

        /*
        var indexG1 = new List<int>();
        var indexG2 = new List<int>();

        foreach (var pr in pressetsG1)
        {
            indexG1.Add(stmc.stamps.FindIndex(s => s.Label == pr));
        }

        foreach (var pr in pressetsG2)
        {
            indexG2.Add(stmc.stamps.FindIndex(s => s.Label == pr));
        }*/

        var rooms = (StampTileMapChromosome.TileMap.GetData() as LBSSchemaData).GetRooms();
        var tiles = rooms.SelectMany(r => r.TilesPositions);

        float fitness = 0;

        Vector2Int offset = new Vector2Int
        (
            tiles.Min(t => t.x),
            tiles.Min(t => t.y)
        );

        foreach (var r in rooms)
        {

            float counterG1 = 0;
            float counterG2 = 0;

            foreach (var tp in r.TilesPositions)
            {
                int val = data[stmc.ToIndex(tp - offset)];
                if (val == -1) continue;
                if (pressetsG1.Contains(stmc.stamps[val].Label))
                {
                    counterG1++;
                }
                if (pressetsG2.Contains(stmc.stamps[val].Label))
                {
                    counterG2++;
                }
                /*
                if (indexG1.Contains(val))
                {
                    counterG1++;
                }
                else if(indexG2.Contains(val))
                {
                    counterG2++;
                }*/
            }

            float p = 0;

            if ((counterG1 == 0 || counterG2 == 0))
            {
                p = MinValue; // Temporal Fix, Should be changed
                //p = counterG1 != counterG2 ? MinValue : MaxValue;
            }
            else
            {
                p = counterG1 / counterG2;
            }
            p = p > MaxValue ? MaxValue / p : p;
            fitness += p;
        }

        return fitness/rooms.Count;
    }

    /// <summary>
    /// Gets the name of the evaluator.
    /// </summary>
    /// <returns> The name of the evaluator. </returns>
    public string GetName()
    {
        return "Tag Proportion by room";
    }
}
