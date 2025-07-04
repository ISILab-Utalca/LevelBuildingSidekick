using Commons.Optimization.Evaluator;
using LBS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IContextualEvaluator : IEvaluator
{
    public List<LBSLayer> ContextLayers { get; set; }

    public void InitializeDefaultWithContext(List<LBSLayer> contextLayers);

    public LBSLayer InteriorLayers()
    {
        throw new NotImplementedException();
    }
}
