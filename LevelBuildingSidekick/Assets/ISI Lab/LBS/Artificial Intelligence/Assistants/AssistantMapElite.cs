using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Optimization.Evaluator;
using ISILab.AI.Categorization;
using ISILab.AI.Optimization;
using ISILab.Extensions;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
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
                var corners = OwnerLayer.ToFixedPosition(RawToolRect.min, RawToolRect.max);

                var size = corners.Item2 - corners.Item1 + Vector2.one;
                return new Rect(corners.Item1, size);
            }
        }

        [JsonIgnore]
        public bool Finished => mapElites.Finished;

        public bool Running => mapElites.Running;

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
        private List<LBSTag> blacklist;

        #endregion

        #region CONSTRUCTORS
        public AssistantMapElite(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }
        #endregion

        #region METHODS

        public override void OnGUI()
        {
        }

        public void Execute()
        {
            toUpdate.Clear();
            mapElites.OnSampleUpdated += (v) => {
                if (!toUpdate.Contains(v))
                {
                    toUpdate.Add(v);
                }
            };
            mapElites.Run();
        }

        public void Continue()
        {
            throw new NotImplementedException(); // TODO: Implement Continue method for AssistantMapElite class
        }

        public void ApplySuggestion(object data)
        {
            var chrom = data as BundleTilemapChromosome;

            if (chrom == null)
            {
                throw new Exception("[ISI Lab] Data " + data.GetType().Name + " is not LBSChromosome!");
            }

            var population = OwnerLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;

            var rect = chrom.Rect;

            for (int i = 0; i < chrom.Length; i++)
            {
                var pos = chrom.ToMatrixPosition(i) + rect.position.ToInt();
                population.RemoveTileGroup(pos);
                var gene = chrom.GetGene(i);
                if (gene == null)
                    continue;
                population.AddTileGroup(pos, gene as BundleData);
            }
        }

        public void LoadPresset(MAPElitesPreset presset)
        {
            if (presset == null)
            {
                throw new Exception("[ISI Lab]: Map Elite Presset not selected or null");
            }
            mapElites = presset.MapElites;
            maskType = presset.MaskType;
            blacklist = presset.blackList;
        }

        public void SetAdam(Rect rect)
        {
            var tm = OwnerLayer.GetModule<BundleTileMap>();
            var chrom = new BundleTilemapChromosome(tm, rect, CalcImmutables(rect));
            mapElites.Adam = chrom;
        }


        private int[] CalcImmutables(Rect rect)
        {
            int[] immutables = null;
            var im = new List<int>();
            var x = (int)rect.min.x;
            var y = (int)rect.min.y;

            if (maskType != null)
            {
                var layers = OwnerLayer.Parent.Layers.Where(l => l.Behaviours.Any(b => b.GetType().Equals(maskType)));
                foreach (var l in layers)
                {
                    var m = l.GetModule<TileMapModule>();

                    if (m == null)
                        continue;

                    for (int j = y; j < y + rect.height; j++)
                    {
                        for (int i = x; i < x + rect.width; i++)
                        {
                            var t = m.GetTile(new Vector2Int(i, j));
                            if (t != null)
                            {
                                continue;
                            }

                            var pos = new Vector2(i, j) - rect.position;
                            var index = (int)(pos.y * rect.width + pos.x);
                            im.Add(index);
                        }
                    }

                }
            }

            var tm = OwnerLayer.GetModule<BundleTileMap>();
            foreach (var g in tm.Groups)
            {
                foreach (var t in g.TileGroup)
                {
                    if (rect.Contains(t.Position))
                    {
                        var characteristics = g.BundleData.Characteristics.Where(c => c is LBSTagsCharacteristic);

                        if (characteristics.Count() == 0)
                            continue;

                        var tags = characteristics.Select(c => (c as LBSTagsCharacteristic).Value);

                        bool flag = false;
                        foreach (var tag in tags)
                        {
                            if (blacklist.Contains(tag))
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag)
                        {
                            var pos = t.Position - rect.position;
                            var i = (int)(pos.y * rect.width + pos.x);
                            im.Add(i);
                        }
                    }
                }
            }


            immutables = im.ToArray();
            return immutables;
        }

        public Texture2D GetBackgroundTexture(Rect rect)
        {
            var text = new Texture2D((int)rect.width, (int)rect.height);


            return text;
        }

        public override object Clone()
        {
            return new AssistantMapElite(Icon, Name, ColorTint);
        }

        public override bool Equals(object obj)
        {
            var other = obj as AssistantMapElite;

            if (other == null) return false;

            if (!this.Name.Equals(other.Name)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}