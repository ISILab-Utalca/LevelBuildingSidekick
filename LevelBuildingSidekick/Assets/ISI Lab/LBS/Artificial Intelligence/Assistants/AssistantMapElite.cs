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
using System.Xml.Linq;

[System.Serializable]
[RequieredModule(typeof(BundleTileMap))]
public class AssistantMapElite : LBSAssistant
{
    MapElites mapElites;
    public Rect Rect { get; set; }

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

    public AssistantMapElite(Texture2D icon, string name) : base(icon, name)
    {
        mapElites = new MapElites();
    }

    public AssistantMapElite(MapElites mapElites, Texture2D icon, string name) : base(icon, name)
    {
        this.mapElites = mapElites;
    }

    public override void Execute()
    {
        //(mapElites.XEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        //(mapElites.YEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        //(mapElites.Optimizer.Evaluator as EvaluatorVE).Init();// IS WRONG Check !!
        mapElites.Run();
    }

    public void Continue() 
    {
        throw new NotImplementedException();
    }

    public void ApplySuggestion(object data)
    {
        var chrom = data as LBSChromosome;

        if (chrom == null)
        {
            throw new Exception("[ISI Lab] Data " + data.GetType().Name + " is not LBSChromosome!");
        }

        var population = Owner.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;

        var rect = chrom.Rect;

        for(int j = 0; j < rect.height; j++)
        {
            for (int i = 0; i < rect.width; i++)
            {
                population.RemoveTile(new Vector2(i,j).ToInt());
            }
        }

        for(int i = 0; i < chrom.Length; i++)
        {
            var pos = chrom.ToMatrixPosition(i) + rect.position.ToInt();
            population.RemoveTile(pos);
            var gene = chrom.GetGene(i);
            if (gene == null)
                continue;
            population.AddTile(pos, gene as BundleData);
        }
    }

    public void LoadPresset(MAPElitesPresset presset)
    {
        mapElites = presset.MapElites;
    }

    public void SetAdam(Rect rect)
    {
        var tm = Owner.GetModule<BundleTileMap>();
        var chrom = new BundleTilemapChromosome(tm, rect);
        mapElites.Adam = chrom;
    }

    public override object Clone()
    {
        return new AssistantMapElite(this.Icon,this.Name);
    }

}


