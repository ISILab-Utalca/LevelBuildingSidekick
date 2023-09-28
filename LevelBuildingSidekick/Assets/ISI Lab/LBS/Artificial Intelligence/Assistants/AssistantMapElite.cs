using LBS.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using LBS.Assisstants;
using System.Xml.Linq;

[System.Serializable]
[RequieredModule(typeof(BundleTileMap))]
public class AssistantMapElite : LBSAssistant
{
    #region FIELDS
    [JsonIgnore]
    private MapElites mapElites = new MapElites();
    [JsonIgnore]
    public List<Vector2> toUpdate = new List<Vector2>();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public Rect Rect { get; set; }
    [JsonIgnore]
    public bool Finished => mapElites.Finished;
    [JsonIgnore]
    public int SampleWidth
    {
        get => mapElites.XSampleCount;
        set => mapElites.XSampleCount = value;
    }
    [JsonIgnore]
    public int SampleHeight
    {
        get => mapElites.YSampleCount;
        set => mapElites.YSampleCount = value;
    }
    [JsonIgnore]
    public IOptimizable[,] Samples => mapElites.BestSamples;
    #endregion

    #region CONSTRUCTORS
    public AssistantMapElite(Texture2D icon, string name) : base(icon, name)
    {
    }

    public AssistantMapElite(MapElites mapElites, Texture2D icon, string name) : base(icon, name)
    {
        this.mapElites = mapElites;
    }
    #endregion

    #region METHODS
    public override void Execute()
    {
        //(mapElites.XEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        //(mapElites.YEvaluator as EvaluatorVE).Init();// IS WRONG Check !!
        //(mapElites.Optimizer.Evaluator as EvaluatorVE).Init();// IS WRONG Check !!
        toUpdate.Clear();
        mapElites.OnSampleUpdated += (v) => { if (!toUpdate.Contains(v)) toUpdate.Add(v); };
        mapElites.Run();
    }

    public void Continue() 
    {
        throw new NotImplementedException();
    }

    public void ApplySuggestion(object data)
    {
        var chrom = data as BundleTilemapChromosome;

        if (chrom == null)
        {
            throw new Exception("[ISI Lab] Data " + data.GetType().Name + " is not LBSChromosome!");
        }

        var population = Owner.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;

        var rect = chrom.Rect;

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

    public override bool Equals(object obj)
    {
        var other = obj as AssistantMapElite;
        
        if (other == null) return false;

        if(!this.Name.Equals(other.Name)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}


