using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using System.Linq;
using Utility;

public class UniformEvaluator : IRangedEvaluator
{

    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;


    //List<StampPresset> pressets = DirectoryTools.GetScriptables<StampPresset>();
    /// <summary>
    /// Evaluates the given evaluable object.
    /// </summary>
    /// <param name="evaluable">The evaluable object to evaluate.</param>
    /// <returns>A float value representing the evaluation of the given object.</returns>
    public float Evaluate(IOptimizable evaluable)
    {
        /*
        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        // var pressets = DirectoryTools.GetScriptables<StampPresset>();

        var data = stmc.GetGenes<int>();
        var total = stmc.stamps.Count;
        var list = stmc.stamps;

        //var rooms = (StampTileMapChromosome.TileMap.GetData() as LBSSchemaData).GetRooms();
        var tiles = rooms.SelectMany(r => r.TilesPositions);

        Vector2Int offset = new Vector2Int
        (
            tiles.Min(t => t.x),
            tiles.Min(t => t.y)
        );

        float Fitness = 0;

        List<int[]> instances = new List<int[]>();
        int[] ocurrences = new int[total];
        foreach (var room in rooms)
        {
            var i = rooms.FindIndex(s => s.ID == room.ID);
            var aux = 0;
            foreach (var tile in room.TilesPositions)
            {
                int val = data[stmc.ToIndex(tile - offset)];
                if (val == -1) continue;

                ocurrences[val]++;
                aux++;
            }
            instances.Add(ocurrences);
        }

        int population_rooms = rooms.Count;

        foreach (var room in instances)
        {
            float temp = 0;
            var id_stamp = 0;
            foreach (var id in room)
            {
                if (id == 0) continue;

                var i = stmc.stamps[id_stamp].Label;
                var presset = pressets.Find(p => p.Label == i);
                var pref = presset.Prefabs.Count;

                id_stamp++;

                float prop = id / pref;
                if (prop > 1) { prop = 1 / prop; }

                temp += prop;

            }

            temp /= id_stamp;

            if (temp == 0) { population_rooms--; }
            Fitness += temp;


        }

        Fitness /= population_rooms;

        Debug.Log("Final: " + Fitness);

        return Mathf.Clamp(Fitness, MinValue, MaxValue);
        */
        return 0;
    }

    public string GetName()
    {
        return "Uniform Evaluator";
    }

    /// <summary>
    /// Creates a visual element for the current object.
    /// </summary>
    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        content.Add(v2);

        return content;
    }

    public float EvaluateH<u>(IOptimizable evaluable, u Heu)
    {
        throw new System.NotImplementedException();
    }
}
