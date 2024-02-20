using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    public class Dispersion
    {
        public float MaxValue => 1;

        public float MinValue => 0;

        int clusterCount = 1;

        public int ClusterCount
        {
            get => clusterCount;
            set => clusterCount = value;
        }

        public float Evaluate(IOptimizable evaluable)
        {
            return 0; // TODO: Implement Dispersion.Evaluate
        }
    }
}