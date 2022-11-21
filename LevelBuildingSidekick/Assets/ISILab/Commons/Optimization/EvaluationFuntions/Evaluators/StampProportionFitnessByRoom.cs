using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using LBS.Representation.TileMap;

public class StampProportionFitnessByRoom : IRangedEvaluator
{
    public StampPresset stamp1;
    public StampPresset stamp2;

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

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
        of1.RegisterCallback<ChangeEvent<StampPresset>>((e) => stamp1 = e.newValue);


        ObjectField of2 = new ObjectField("Stamp 2: ");
        of2.objectType = typeof(StampPresset);
        of2.value = stamp2;
        of2.RegisterCallback<ChangeEvent<StampPresset>>((e) => stamp2 = e.newValue);

        content.Add(v2);
        content.Add(of1);
        content.Add(of2);

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

        var p1count = stmc.stamps.Select(s => s.Label == stamp1.Label).Count();
        var p2count = stmc.stamps.Select(s => s.Label == stamp2.Label).Count();

        if (p1count == 0 || p2count == 0)
        {
            if(p1count == 0 && p2count == 0)
            {
                return MaxValue;
            }

            return MinValue;
        }

        var rooms = (StampTileMapChromosome.TileMap.GetData() as LBSSchemaData).GetRooms();

        var fitness = 0;

        foreach (var r in rooms)
        {
            int counterP1 = 0;
            int counterP2 = 0;
            foreach (var t in r.Tiles)
            {
                int val = stmc.GetGene<int>(stmc.ToIndex(t.GetPosition()));
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
            var c = counterP1 / counterP2;
            c = c > 1 ? 1 / c : c;
            fitness += c;
        }

        return fitness/rooms.Count;
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }
}
