using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GroupCount : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    float distThreshold = 1;
    public float DistThreshold
    {
        get => distThreshold;
        set => distThreshold = value;
    }

    DistanceType distType;
    public DistanceType DistType 
    { 
        get => distType; 
        set => distType = value; 
    }

    List<Object> whiteList = new List<Object>();
    public List<Object> WhiteList
    {
        get => whiteList;
        set => whiteList = value;
    }


    public float Evaluate(IOptimizable evaluable)
    {
        float fitness = 0;

        var chr = evaluable as LBSChromosome;
        var genes = chr.GetGenes();

        var toRemove = genes.Select((g, i) => new { g, i }).Where(x => x.g != null && whiteList.Contains(x.g)).Select(x => x.i).ToList();

        float max = (chr.Length - chr.ImmutablesCount - toRemove.Count) / ((distThreshold + 1) * (distThreshold + 1));

        var indexes = new List<int>();


        for (int i = 0; i < genes.Length; i++)
        {
            if (genes[i] != null && !toRemove.Contains(i))
                indexes.Add(i);
        }

        if (indexes.Count == 0)
            return MinValue;

        int groups = 0;

        while(indexes.Count > 0)
        {
            var connected = Sweep(indexes[0], indexes, chr);
            if (connected.Count > 0)
                groups++;
            foreach(var c in connected)
            {
                indexes.Remove(c);
            }
        }

        fitness = groups / max;


        return MinValue + (MaxValue - MinValue) * fitness;
    }

    private List<int> Sweep(int index, List<int> indexes, LBSChromosome chrom)
    {
        List<int> closed = new List<int>();
        Queue<int> open = new Queue<int>();
        open.Enqueue(index);
        while(open.Count > 0)
        {
            var parent = open.Dequeue();
            closed.Add(parent);
            var pos = chrom.ToMatrixPosition(parent);
            var children = new List<int>();
            for (float j = pos.y - distThreshold; j <= pos.y + distThreshold; j++)
            {
                for (float i = pos.x - distThreshold; i <= pos.x + distThreshold; i++)
                {
                    var c_pos = new Vector2(i, j);
                    if ((c_pos - pos).Distance(distType) > distThreshold)
                    {
                        continue;
                    }

                    var id = chrom.MatrixToIndex(c_pos);
                    if (indexes.Contains(id))
                    {
                        children.Add(id);
                    }
                }
            }

            foreach(var c in children)
            {
                if(!closed.Contains(c) && !open.Contains(c))
                {
                    open.Enqueue(c);
                }
            }

        }

        return closed;
    }
}
