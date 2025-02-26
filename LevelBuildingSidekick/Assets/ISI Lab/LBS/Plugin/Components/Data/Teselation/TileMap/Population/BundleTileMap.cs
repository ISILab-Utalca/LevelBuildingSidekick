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
                AddGroupTiles(t);
            }
        }
        #endregion

        #region METHODS
        //METHODS TO REPLACE ELSEWHERE
        //AddTile -> CreateGroup
        //GetTIle -> GetGroup
        //RemoveTile -> RemoveGroup

        //For a bundle to add a tile, it should always add the entire group.
        //Check bundle size -> create group -> check tile location and add all tiles according to size.
        
        //Screw it, let's make the whole thing here lol
        public void CreateGroup(Vector2Int position, BundleData bundleData, Vector2 rotation, bool addGroup = true) {

            //Create group, then get the tilesize
            TileBundleGroup newGroup = new TileBundleGroup(new List<LBSTile>(), bundleData, rotation);
            Vector2Int groupSize = newGroup.GetBundleSize();
            
            //Fill group with tiles according to tilesize
            for(int i=0; i<groupSize.x; i++)
            {
                for(int j=0; j<groupSize.y; j++)
                {
                    newGroup.TileGroup.Add( new LBSTile( new Vector2( position.x + i, position.y + j ) ) );
                }
            }
            //This creates the group, now all that's left is to add it.

            if (addGroup) { AddGroupTiles(newGroup); }

            //TODO: I need a method that checks a tile and checks if it's part of any group already active. Will do it after the one that adds the group.
        }
        
        //Executed alongside the one that creates a group by default
        public void AddGroupTiles(TileBundleGroup group)
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

        public TileBundleGroup GetGroup(Vector2 pos)
        {
            if (groups.Count <= 0)
                return null;
            //return groups.Find(t => t.Tiles.Position == pos);

            //Find the position and return the group if it isn't null
            foreach (TileBundleGroup group in groups)
            {
                LBSTile searchTile = group.TileGroup.Find(t => t.Position == pos);
                if (searchTile != null) { return group; }
            }
            return null;
        }

        //Remove group from group
        public void RemoveGroup(TileBundleGroup group)
        {
            if (groups.Count <= 0) return;

            OnChanged?.Invoke(this, new List<object>() { group }, null);
            groups.Remove(group);
        }

        public void RemoveGroup(LBSTile tile)
        {
            TileBundleGroup groupToRemove = GetGroup(tile);
            if (groupToRemove != null) { RemoveGroup(groupToRemove); }

            return;
        }

        public void RemoveGroup(int index) => RemoveGroup(groups[index]);

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
                AddGroupTiles(t);
            }
        }

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
        public Vector2Int GetBundleSize()
        {
            return bData.Bundle.TileSize;
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

