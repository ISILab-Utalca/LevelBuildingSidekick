using LBS.AI;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using System.Linq;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Populations;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Terminations;
using LBS.Components.Graph;
using LBS.Tools.Transformer;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using LBS.Assisstants;

[System.Serializable]
[RequieredModule(typeof(TaggedTileMap))]
public class AssistantMapElite : LBSAssistantAI
{
    MapElites mapElites;

    public bool Finished => mapElites.Finished;
    public int SampleWidth
    {
        get => mapElites.XSampleCount;
        set => mapElites.XSampleCount = value;
    }
    public int SampleHeight
    {
        get => mapElites.YSampleCount;
        set => mapElites.YSampleCount = value;
    }

    public IOptimizable[,] Samples => mapElites.BestSamples;

    public AssistantMapElite()
    {
        mapElites = new MapElites();
    }

    public AssistantMapElite(MapElites mapElites)
    {
        this.mapElites = mapElites;
    }

    public override void Execute()
    {
        (mapElites.XEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        (mapElites.YEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        (mapElites.Optimizer.Evaluator as EvaluatorVE).Init();// IS WRONG Check !!
        mapElites.Run();
    }

    public void Continue() 
    {
        
    }

    public override void Init(LBSLayer layer)
    {
    }

    public void ApplySuggestion(object data)
    {
        var chrom = data as LBSChromosome;

        if (chrom == null)
        {
            throw new Exception("[ISI Lab] Data " + data.GetType().Name + " is not LBSChromosome!");
        }

        var modules = chrom.ToModules();

        foreach (var m in modules) //GOTTA CHANGE THIS
        {
            var mod = Owner.GetModule<LBSModule>(m.ID);
            mod.Rewrite(chrom.ToModule());
        }
    }

    public void LoadPresset(MAPElitesPresset presset)
    {
        mapElites.Optimizer = presset.optimizer;
        mapElites.XEvaluator = presset.xEvaluator;
        mapElites.YEvaluator = presset.yEvaluator;
        mapElites.XThreshold = presset.xThreshold;
        mapElites.YThreshold = presset.yThreshold;
        mapElites.XSampleCount = presset.xSampleCount;
        mapElites.YSampleCount = presset.ySampleCount;
        mapElites.devest = presset.devest;
    }

    public void SetAdam(Rect rect)
    {
        throw new Exception("Not implemented");
    }
    public override object Clone()
    {
        return new AssistantMapElite();
    }
}


