using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using LBS.Assisstants;
using System.Linq;
using Commons.Optimization.Evaluator;
using ISILab.Extensions;
using ISILab.LBS.AI.Categorization;

namespace ISILab.LBS.AI.Assistants
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
                var corners = Owner.ToFixedPosition(RawToolRect.min, RawToolRect.max);

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
        private List<LBSIdentifier> blacklist;

        #endregion

        #region CONSTRUCTORS
        public AssistantMapElite(Texture2D icon, string name) : base(icon, name)
        {
        }
        #endregion

        #region METHODS
        public override void Execute()
        {
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

            for (int i = 0; i < chrom.Length; i++)
            {
                var pos = chrom.ToMatrixPosition(i) + rect.position.ToInt();
                population.RemoveTile(pos);
                var gene = chrom.GetGene(i);
                if (gene == null)
                    continue;
                population.AddTile(pos, gene as BundleData);
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
            var tm = Owner.GetModule<BundleTileMap>();
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
                var layers = Owner.Parent.Layers.Where(l => l.Behaviours.Any(b => b.GetType().Equals(maskType)));
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
                                Debug.Log(t.Position);
                                continue;
                            }

                            var pos = new Vector2(i, j) - rect.position;
                            var index = (int)(pos.y * rect.width + pos.x);
                            im.Add(index);
                        }
                    }

                }
            }

            var tm = Owner.GetModule<BundleTileMap>();
            foreach (var b in tm.Tiles)
            {
                if (rect.Contains(b.Tile.Position))
                {
                    var characteristics = b.BundleData.Characteristics.Where(c => c is LBSTagsCharacteristic);

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
                        var pos = b.Tile.Position - rect.position;
                        var i = (int)(pos.y * rect.width + pos.x);
                        im.Add(i);
                    }
                }
            }


            immutables = im.ToArray();
            Debug.Log(immutables.Length);
            return immutables;
        }

        public Texture2D GetBackgroundTexture(Rect rect)
        {
            var text = new Texture2D((int)rect.width, (int)rect.height);


            return text;
        }

        public override object Clone()
        {
            return new AssistantMapElite(this.Icon, this.Name);
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