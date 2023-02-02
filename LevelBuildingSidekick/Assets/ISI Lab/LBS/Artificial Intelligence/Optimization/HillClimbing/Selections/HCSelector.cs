﻿using Commons.Optimization.Evaluator;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class HCSelector<U> : ISelection
{   
    private List<IOptimizable> candidates;
    private U heuristic;
    private double? higherScore;
    private double? newScore;

    public HCSelector(List<IOptimizable> candidates, U heuristic)
    {
        this.candidates = candidates;
        this.heuristic = heuristic;
    }

    public VisualElement CIGUI()
    {
        throw new NotImplementedException();
    }


    public List<IOptimizable> GetBetters(IEvaluator evaluator, double? score)
    {
        List<IOptimizable> betters = new List<IOptimizable>();
        higherScore = score;

        /*for (int i = 0; i < candidates.Count; i++)
        {
            newScore = evaluator.EvaluateH(candidates[i], heuristic);
            if (newScore > higherScore)
            {
                betters.Clear();
                betters.Add(candidates[i]);
            }
            else if (newScore == higherScore)
            {
                betters.Add(candidates[i]);
            }
            higherScore = higherScore < newScore ? newScore : higherScore;
        }*/

        return betters;
    }

    public IList<IOptimizable> SelectEvaluables(int number, Generation generation)
    {
        throw new NotImplementedException();
    }
}