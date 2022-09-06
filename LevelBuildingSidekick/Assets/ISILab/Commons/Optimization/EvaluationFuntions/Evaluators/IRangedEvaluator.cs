using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Optimization.Fitness
{
    public interface IRangedEvaluator : IEvaluator
    {
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
    }
}

