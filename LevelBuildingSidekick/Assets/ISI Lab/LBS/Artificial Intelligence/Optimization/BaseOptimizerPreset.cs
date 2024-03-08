using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Terminations;
using ISILab.AI.Optimization.Populations;
using ISILab.AI.Optimization.Selections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ISILab.AI.Optimization
{
    public class BaseOptimizerPreset : ScriptableObject
    {
        [SerializeField, SerializeReference]
        public IPopulation population;
        [SerializeField, SerializeReference]
        public IEvaluator evaluator;
        [SerializeField, SerializeReference]
        public ISelection selection;
        [SerializeField, SerializeReference]
        public ITermination termination;
    }
}

