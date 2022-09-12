using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Optimization.Fitness
{
    public interface IRangedEvaluator : IEvaluator
    {
        public float MaxValue { get;}
        public float MinValue { get;}
    }
}

