using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Optimization.Evaluator
{
    /// <summary>
    /// Represents a ranged evaluator.
    /// </summary>
    public interface IRangedEvaluator : IEvaluator
    {
        public float MaxValue { get;}
        public float MinValue { get;}

        public float LocalMax { get; }
        public float LocalMin { get; }
    }
}

