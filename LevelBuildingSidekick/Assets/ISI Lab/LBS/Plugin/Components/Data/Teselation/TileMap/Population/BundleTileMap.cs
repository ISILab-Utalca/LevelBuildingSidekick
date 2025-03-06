using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;

namespace ISILab.LBS.Modules
{
    public class BundleTileMap : LBSModule, ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        protected List<TileBundleGroup> groups = new List<TileBundleGroup>();
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<TileBundleGroup> Groups => new List<TileBundleGroup>(groups);
        #endregion

        #region CONSTRUCTORS
        public BundleTileMap() : base()
        {
            id = GetType().Name;
        }

        //This one has to be compatible with groups so let's see how I'll make groups work
        public BundleTileMap(IEnumerable<TileBundleGroup> groups, string id = "ConnectedTileMapModule") : base(id)
        {
            foreach (var t in groups)
            {
                AddGroup(t);
            }
        }
        #endregion

        #region METHODS
        //METHODS TO REPLACE ELSEWHERE
        //AddTile -> CreateGroup
        //GetTIle -> GetGroup
        //RemoveTile -> RemoveGroup

        //METHODS THAT ARE NECESSARY
        // - Create group from a position, bundledata and rotation combo = Check!
        // - Add group to the group list, replacing anything in the way = Check!

        // - Get group from tile = Check!
        // - Get group from location = Check!
        // - Remove group = Check! ( From tile = Check! / From index = Check!)+

        // - Get tile from group = Can be done from group
        // - Get tile from position = Can be done from group

        // - Check if tilemap has tile = Check!
        // - Check if tilemap has group = Check!

        //For a bundle to add a tile, it should always add the entire group.
        //Check bundle size -> create group -> check tile location and add all tiles according to size.

        //Creates group from variables provided
        public TileBundleGroup CreateGroup(Vector2Int position, BundleData bundleData, Vector2 rotation) {

            //Create group, then get the tilesize
            TileBundleGroup newGroup = new TileBundleGroup(new List<LBSTile>(), bundleData, rotation);
            Vector2Int groupSize = newGroup.GetBundleSize();
            
            //Fill group with tiles according to tilesize
            for(int i=0; i<groupSize.x; i++)
            {
                for(int j=0; j<groupSize.y; j++)
                {
                    newGroup.TileGroup.Add( new LBSTile( new Vector2( position.x + i, position.y - j ) ) );
                }
            }

            AddGroup(newGroup);
            return newGroup;
        }
        
        public bool ValidNewGroup(Vector2Int position, BundleData bundleData, Vector2 rotation)
        {
         
            //Create group, then get the tilesize
            TileBundleGroup newGroup = new TileBundleGroup(new List<LBSTile>(), bundleData, rotation);
            Vector2Int groupSize = newGroup.GetBundleSize();
            
            // Check if its valid to add a tilegroup
            foreach (var tbg in groups)
            {
                foreach (var lbsTile in tbg.TileGroup)
                {
                    for(int i=0; i<groupSize.x; i++)
                    {
                        for(int j=0; j<groupSize.y; j++)
                        {
                            if (new Vector2(position.x + i, position.y - j) != lbsTile.Position) continue;
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        
        //Adds a group to the group list and replaces anything in the way
        public void AddGroup(TileBundleGroup group)
        {
            foreach(LBSTile tile in group.TileGroup)
            {
                //Check if there's tiles and remove their respective groups.
                var groupToRemove = GetGroup(tile);
                if (groupToRemove != null)
                {
                    groups.Remove(groupToRemove);
                }
            }
            //OnChanged
            OnChanged?.Invoke(this, null, new List<object>() { group });

            //Then add the group!
            groups.Add(group);
        }

        //Gets a group from any tile
        public TileBundleGroup GetGroup(LBSTile tile)
        {
            //The obvious
            if (groups.Count <= 0)
                return null;

            //Then, find the specific tile and return the group
            foreach (TileBundleGroup group in groups)
            {
                LBSTile searchTile = group.TileGroup.Find(t => t == tile);
                if (searchTile != null) { return group; }
            }
            return null;
        }

        //Gets a group from a position
        public TileBundleGroup GetGroup(Vector2 pos)
        {
            if (groups.Count <= 0)
                return null;

            //Find the position and return the group if it isn't null
            foreach (TileBundleGroup group in groups)
            {
                LBSTile searchTile = group.TileGroup.Find(t => t.Position == pos);
                if (searchTile != null) { return group; }
            }
            return null;
        }

        //Remove group
        public void RemoveGroup(TileBundleGroup group)
        {
            if (groups.Count <= 0) return;
            if (group == null) return;

            OnChanged?.Invoke(this, new List<object>() { group }, null);
            groups.Remove(group);
        }

        //Remove group from tile
        public void RemoveGroup(LBSTile tile) => RemoveGroup(GetGroup(tile));
        
        //Remove group from index
        public void RemoveGroup(int index) => RemoveGroup(groups[index]);
        
        //Check if there's a tile on the tilemap
        public bool Contains(LBSTile tile)
        {
            if (groups.Count <= 0)
                return false;
            foreach (TileBundleGroup group in groups)
            {
                bool tileExists = group.TileGroup.Any(t => t == tile);
                if(tileExists) return true;
            }
            return false;
        }
        //Check if there's a group on the tilemap
        public bool Contains(TileBundleGroup group)
        {
            if(groups.Count <= 0) return false;
            if(groups.Any(t => t == group)) return true;
            return false;
        }
        
        //Idk if this one works, modify if it breaks anything
        public override Rect GetBounds()
        {
            if (groups == null || groups.Count == 0)
            {
                return new Rect(Vector2.zero, Vector2.zero);
            }

            //I have no idea if this works but I want to see because I'm tired of the entire code flashing red!!
            return groups.Select(t => t.TileGroup[0]).GetBounds();
        }

        public override bool IsEmpty()
        {
            return groups.Count <= 0;
        }

        public override void Clear()
        {
            groups.Clear();
        }

        public override object Clone()
        {
            return new BundleTileMap(groups.Select(t => t.Clone()).Cast<TileBundleGroup>(), ID);
        }

        public override void Rewrite(LBSModule module)
        {
            var map = module as BundleTileMap;
            if (map == null)
            {
                return;
            }

            OnChanged?.Invoke(this, new List<object>(groups), new List<object>(map.groups));

            Clear();
            foreach (var t in map.groups)
            {
                AddGroup(t);
            }
        }
        
        //Selects groups, probably needs fixing
        public List<object> GetSelected(Vector2Int position)
        {
            var pos = Owner.ToFixedPosition(position);
            var r = new List<object>();
            var group = GetGroup(pos);

            if (group != null)
            {
                r.Add(group.BundleData);
            }

            return r;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BundleTileMap;

            if (other == null) return false;

            var tCount = other.groups.Count;

            if (tCount != this.groups.Count) return false;

            for (int i = 0; i < tCount; i++)
            {
                var t1 = other.groups[i];
                var t2 = this.groups[i];

                if (!t1.Equals(t2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

   
    }

    /// <summary>
    /// So TileBundlePairs are a little too limited in functionality since they only link a tile to bundle data and that's it.
    /// I made a reworked version that SHOULD allow for multiple tiles to be stored with an ID.
    /// </summary>
    [System.Serializable]
    public class TileBundleGroup : ICloneable
    {
        [SerializeField, JsonRequired]
        List<LBSTile> tileGroup = new List<LBSTile>();
        [SerializeField, JsonRequired]
        BundleData bData;
        [SerializeField, JsonRequired]
        Vector2 rotation;

        #region PROPERTIES
        [JsonIgnore]
        public List<LBSTile> TileGroup
        {
            get => tileGroup;
            set => tileGroup = value;
        }

        [JsonIgnore]
        public BundleData BundleData
        {
            get => bData;
            set => bData = value;
        }

        [JsonIgnore]
        public Vector2 Rotation
        {
            get => rotation;
            set => rotation = value;
        }
        public TileBundleGroup(List<LBSTile> tiles, BundleData bData, Vector2 rotation)
        {
            this.tileGroup = tiles;
            this.bData = bData;
            this.rotation = rotation;
        }
        #endregion

        #region METHODS
        //Returns bundle size
        public Vector2Int GetBundleSize()
        {
            return bData.Bundle.TileSize;
        }

        //Check if group contains tile
        public bool Contains(LBSTile tile)
        {
            if (tileGroup.Count <= 0) return false;
            if (tileGroup.Any(t => t == tile)) return true;
            return false;
        }

        //Get tile from position
        public LBSTile GetTile(Vector2Int pos)
        {
            return TileGroup.Find(t => t.Position == pos);
        }

        public TileBundleGroup FindFromTile(LBSTile tile)
        {
            if(this.Contains(tile)) return this;
            return null;
        }

        //Get tile
        public LBSTile GetTile(LBSTile tile)
        {
            return TileGroup.Find(t => t == tile);
        }

        public Rect GetBounds()
        {
            var x = TileGroup.Min(t => t.Position.x);
            var y = TileGroup.Max(t => t.Position.y);

            var width = TileGroup.Max(t => t.Position.x) - x + 1;
            var height = TileGroup.Max(t => t.Position.y) - y + 1;

            return new Rect(x, y, width, height);
        }

        public Vector2 GetCenter()
        {
            var bounds = this.GetBounds();
            return new Vector2(
                (bounds.x + bounds.width/2),
                (bounds.y + bounds.height/2)
                );
        }

        public object Clone()
        {
            return new TileBundleGroup(tileGroup.Clone(), bData.Clone() as BundleData, rotation);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileBundleGroup;

            if (other == null) return false;

            if (tileGroup.Count != other.tileGroup.Count) return false;

            for (int i = 0; i < tileGroup.Count; i++)
            {
                if (!tileGroup[i].Equals(other.tileGroup[i])) return false;
            }

            if (!this.bData.Equals(other.bData)) return false;

            if (!this.rotation.Equals(other.rotation)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}

