using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using LBS.Representation.TileMap;
using System;

public class StampProportionFitnessByRoom : IRangedEvaluator
{
    public StampPresset stamp1;
    public StampPresset stamp2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

    /// <summary>
    /// Initializes a new instance of the <see cref="StampProportionFitnessByRoom"/> class.
    /// </summary>
    public StampProportionFitnessByRoom()
    {
        var t = Utility.DirectoryTools.GetScriptables<StampPresset>();
        stamp1 = t.First();
        stamp2 = t.Last();
    }

    /// <summary>
    /// Creates the GUI for the evaluator.
    /// </summary>
    /// <returns> A <see cref="VisualElement"/> containing the GUI. </returns>
    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        ObjectField of1 = new ObjectField("Stamp 1: ");
        of1.objectType = typeof(StampPresset);
        of1.value = stamp1;
        of1.RegisterValueChangedCallback(e => stamp1 = e.newValue as StampPresset);

        ObjectField of2 = new ObjectField("Stamp 2: ");
        of2.objectType = typeof(StampPresset);
        of2.value = stamp2;
        of2.RegisterValueChangedCallback(e => stamp2 = e.newValue as StampPresset);

        content.Add(v2);
        content.Add(of1);
        content.Add(of2);

        return content;
    }

    /// <summary>
    /// Compare the presence of one stamp with another in each of the rooms.
    /// </summary>
    /// <param name="evaluable"></param>
    /// <returns> A float value indicating the fitness of the chromosome. </returns>
    public float Evaluate(IOptimizable evaluable)
    {
        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        var data = stmc.GetGenes<int>();

        var foundS1 = stmc.stamps.Any(s => s.Label == stamp1.Label);
        var founsS2 = stmc.stamps.Any(s => s.Label == stamp2.Label);

        if (!foundS1 || !founsS2)
        {
            return MinValue;// Temporal Fix, Should be changed
            //return foundS1 != founsS2 ? MinValue : MaxValue;
        }

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
            float counterP1 = 0;
            float counterP2 = 0;
            foreach (var tp in r.TilesPositions)
            {
                int val = data[stmc.ToIndex(tp - offset)];
                if (val == -1) continue;
                if (stamp1.Label == stmc.stamps[val].Label)
                {
                    counterP1++;
                }
                if (stamp2.Label == stmc.stamps[val].Label)
                {
                    counterP2++;
                }
            }

            float p = 0;

            if ((counterP1 == 0 || counterP2 == 0))
            {
                p = MinValue;// Temporal Fix, Should be changed
                //p = counterP1 != counterP2 ? MinValue : MaxValue;
            }
            else
            {
                 p = counterP1 / counterP2;
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
        return "Stamp proportion fitness by room";
    }

    public float EvaluateH<u>(IOptimizable evaluable, u Heu)
    {
        throw new NotImplementedException();
    }
}
