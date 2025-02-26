using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [System.Serializable]
    [Obsolete("??")]
    public class TaggedTileMap : LBSModule
    {
        [SerializeField, JsonRequired, SerializeReference]
        private List<TileBundleGroup> tileGroups = new List<TileBundleGroup>();

        [JsonIgnore]
        public List<TileBundleGroup> TileGroups => tileGroups;

        public Func<LBSTile, bool> OnRemoveTile { get; private set; }

        [JsonIgnore]
        protected Func<LBSTile, bool> OnAddTile;

        public TaggedTileMap() : base()
        {
            ID = GetType().Name;
        }

        public TaggedTileMap(string key, List<TileBundleGroup> groups) : base(key)
        {
            this.tileGroups = groups;
        }

        public override void Clear()
        {
            for (int i = 0; i < tileGroups.Count; i++)
            {
                foreach(var tile in tileGroups[i].TileGroup) { 
                    OnRemoveTile?.Invoke(tile);
                }
            }
            tileGroups.Clear();
        }

        public BundleData GetBundleData(LBSTile tile)
        {
            var t = tileGroups.Find(g => g.HasTile(tile));
            return t.BundleData;
        }

        //This does nothing? But I'll change it for addGroup.
        public void AddGroup(List<LBSTile> tiles, Bundle bundle)
        {
            var data = new BundleData(bundle.name, bundle.Characteristics);
            AddGroup(tiles, data);
        }

        public void AddGroup(List<LBSTile> tiles, BundleData data)
        {
            var t = tileGroups.Find(g => g.TileGroup == tiles);

            if (t == null)
            {
                foreach (var groupTile in t.TileGroup)
                {
                    OnAddTile?.Invoke(groupTile);
                }

                t = tileGroups.Find(g => g.TileGroup == tiles);
            }
            t.BundleData = data;
        }

        public override object Clone()
        {
            var dir = new List<TileBundleGroup>();
            foreach (var group in TileGroups)
            {
                dir.Add(new TileBundleGroup(group.TileGroup, group.BundleData, group.Rotation));
            }

            return new TaggedTileMap(id, dir);
        }

        public override bool IsEmpty()
        {
            return tileGroups.Count == 0;
        }

        public override void OnAttach(LBSLayer layer)
        {
            var tileMap = layer.GetModule<TileMapModule>();
            //OnAddTile += tileMap.AddTile;
            OnRemoveTile += tileMap.RemoveTile;
        }

        public override void OnDetach(LBSLayer layer)
        {
            var tileMap = layer.GetModule<TileMapModule>();
            //OnAddTile -= tileMap.AddTile;
            OnRemoveTile -= tileMap.RemoveTile;
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveGroup(object tile)
        {
            var toR = tile as LBSTile;
            var xx = tileGroups.Find(x => x.HasTile(toR));
            tileGroups.Remove(xx);
        }

        public void AddEmpty(object tile)
        {
            var t = tile as LBSTile;
            var xx = tileGroups.Find(x => x.HasTile(t));
            if (xx != null)
            {
                xx.BundleData = null;
                return;
            }
            tileGroups.Add(new TileBundleGroup(new List<LBSTile> { t }, null, Vector2.right));
        }

        public override Rect GetBounds()
        {
            var x = tileGroups.Min(p => p.TileGroup.Min(t => t.Position.x));
            var y = tileGroups.Min(p => p.TileGroup.Min(t => t.Position.y));

            var width = tileGroups.Max(p => p.TileGroup.Max(t => t.Position.x)) - x + 1;
            var height = tileGroups.Max(p => p.TileGroup.Max(t => t.Position.y)) - y + 1;

            return new Rect(x, y, width, height);
        }

        public override void Rewrite(LBSModule module)
        {
            var tileMap = module as TaggedTileMap;

            if (tileMap == null)
            {
                throw new Exception("[ISI Lab] Modules have to be of the same type!");
            }

            Clear();

            foreach (var g in tileMap.TileGroups)
            {
                AddGroup(g.TileGroup, g.BundleData);
            }
        }

        public override void Reload(LBSLayer layer)
        {
            OnAttach(layer);
        }
    }
}