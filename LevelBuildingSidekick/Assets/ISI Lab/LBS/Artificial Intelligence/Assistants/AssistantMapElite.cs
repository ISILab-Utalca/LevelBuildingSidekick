using LBS.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using LBS.Assisstants;
using System.Xml.Linq;
using System.Linq;
using Commons.Optimization.Evaluator;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    public Rect RawToolRect { get; set; }

    [JsonIgnore]
    public Rect Rect
    {
        get
        {
            var corners = Owner.ToFixedPosition(RawToolRect.min, RawToolRect.max);

            var size = corners.Item2 - corners.Item1 + Vector2.one; 
            return new Rect(corners.Item1, size);
        }
    }

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

    [JsonIgnore]
    public IEvaluator XEvaluator => mapElites.XEvaluator;

    [JsonIgnore]
    public IEvaluator YEvaluator => mapElites.YEvaluator;

    private Type maskType;

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
        maskType = presset.MaskType;
    }

    public void SetAdam(Rect rect)
    {
        var tm = Owner.GetModule<BundleTileMap>();
        var chrom = new BundleTilemapChromosome(tm, rect, CalcImmutables(rect));
        mapElites.Adam = chrom;
    }

    private int[] CalcImmutables(Rect rect)
    {
        int[] immutables = null;
        if (maskType != null)
        {
            var layers = Owner.Parent.Layers.Where(l => l.Behaviours.Any(b => b.GetType().Equals(maskType)));
            var im = new List<int>();
            foreach (var l in layers)
            {
                var m = l.GetModule<TileMapModule>();

                if (m == null)
                    continue;

                var tiles = m.Tiles;

                foreach (var t in tiles)
                {
                    if (!rect.Contains(t.Position))
                        continue;
                    var pos = t.Position - rect.position;
                    var i = (int)(pos.y * rect.width + pos.x);
                    im.Add(i);
                }

            }
            immutables = im.ToArray();
        }

        return immutables;
    }

    public Texture2D GetBackgroundTexture(Rect rect)
    {
        var text = new Texture2D((int)rect.width, (int)rect.height);


        return text;
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


