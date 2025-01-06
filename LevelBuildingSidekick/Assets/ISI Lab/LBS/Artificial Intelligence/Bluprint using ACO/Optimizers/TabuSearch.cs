using Optimization.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabuSearch 
{
    private Func<List<Map>, List<Map>> Search;
    private Func<Map, float> Evaluate; // normalizado
    private Func<bool> Terminator;

    public Map Ejecute(Map init, Func<Map, float> evaluate, Func<List<Map>, List<Map>> saerch)
    {
        Map best = init;
        float bestFitness = Evaluate.Invoke(best);
        this.Search = saerch;
        this.Evaluate = evaluate;

        var tabu = new List<Map>();

        while(!Terminator.Invoke())
        {
            var neighbor = GetNeighbors(best, tabu);

            var selected = Search.Invoke(neighbor);

            for (int i = 0; i < selected.Count; i++)
            {
                var newFitness = Evaluate.Invoke(selected[i]);

                var diff = newFitness - bestFitness;

                if (diff > 0)
                {
                    best = selected[i];
                    bestFitness = newFitness;
                }
            }
        }

        return best;
    }

    public List<Map> GetNeighbors(Map map, List<Map> tabu)
    {
        return new List<Map>(); // TODO: Implement
    }
}
