using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule), typeof(BundleTileMap))]
    public class PopulationBehaviour : LBSBehaviour
    {
        #region FIELDS
        [SerializeField, JsonIgnore]
        TileMapModule tileMap;
        [SerializeField, JsonIgnore]
        BundleTileMap bundleTileMap;

        [SerializeField,JsonRequired]
        private string bundleRefGui = "3e607c0f80297b849a6ea0d7f98c73a3";
        
        private HashSet<TileBundleGroup> _newRotations = new ();
        #endregion

        #region META-FIELDS
        [JsonIgnore]
        public Bundle selectedToSet;
        
        [SerializeField, JsonIgnore]
        private BundleCollection bundleCollection;
        
        public string allFilter = "All";
        
        [FormerlySerializedAs("selectedTypetoSet")] [JsonIgnore]
        public string selectedTypeFilter;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<TileBundleGroup> Tilemap => bundleTileMap.Groups;
        
        [JsonIgnore]
        public BundleTileMap BundleTilemap => bundleTileMap;
        
        public BundleCollection BundleCollection 
        {
            get => GetBundleCollection();
            set
            {
                bundleCollection = value;
                bundleRefGui = LBSAssetMacro.GetGuidFromAsset(value);
            }
        }
        
        public string SelectedFilter 
        {
            get => GetFilter();
            set
            {
                selectedTypeFilter = value;
            }
        }

        private string GetFilter()
        {
            return selectedTypeFilter ?? allFilter;
        }

        #endregion

        #region CONSTRUCTORS
        public PopulationBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint) { }
        #endregion

        #region METHODS
        
        public override void OnGUI()
        {

        }
        public void AddTileGroup(Vector2Int position, Bundle bundle) => AddTileGroup(position, new BundleData(bundle));
        public void AddTileGroup(Vector2Int position, BundleData bundle)
        {
            if (!bundleTileMap.ValidNewGroup(position, bundle, Vector2.right)) return;
            
            //Create group
            var group = bundleTileMap.CreateGroup(position, bundle, Vector2.right);

            //Add all tiles from the group
            foreach(LBSTile tile in group.TileGroup)
            {
                tileMap.AddTile(tile);
            }
            RequestTilePaint(group);
        }

        public void RemoveTileGroup(Vector2Int position)
        {
            var tile = tileMap.GetTile(position);
            var group = bundleTileMap.GetGroup(position);

            //CHANGE FROM HERE
            if (group != null)
            {
                foreach (var groupTile in group.TileGroup)
                {
                    tileMap.RemoveTile(groupTile);
                }
                bundleTileMap.RemoveGroup(group);
                RequestTileRemove(group);
            }
        }
        public bool ValidNewGroup(Vector2Int position, Bundle bundle)
        {
            return bundleTileMap.ValidNewGroup(position, new BundleData(bundle), Vector2.right);
        }

        public bool ValidMoveGroup(Vector2Int position, TileBundleGroup group)
        {
            return bundleTileMap.ValidMoveGroup(position, group, Vector2.right);
        }
        
        public void SetBundle(TileBundleGroup group, Bundle bundle)
        {
            group.BundleData = new BundleData(bundle);
            ReplaceTile(group);
        }
        public void SetBundle(LBSTile tile, Bundle bundle) => SetBundle(bundleTileMap.GetGroup(tile), bundle);

        public LBSTile GetTile(Vector2Int position)
        {
            return tileMap.GetTile(position);
        }

        public TileBundleGroup GetTileGroup(Vector2Int position)
        {
            return bundleTileMap.GetGroup(position);
        }

        public BundleData GetBundleData(LBSTile tile)
        {
            return bundleTileMap.GetGroup(tile).BundleData;
        }

        public void RotateTile(Vector2Int pos, Vector2 rotation)
        {
            TileBundleGroup t = GetTileGroup(pos);
            if (t == null)
                return;
            t.Rotation = rotation;

            _newRotations.Add(t);
        }

        public Vector2 GetTileRotation(Vector2Int pos)
        {
            TileBundleGroup t = GetTileGroup(pos);
            return t == null ? default : t.Rotation;
        }

        public BundleData GetBundleData(Vector2 position)
        {
            return GetBundleData(tileMap.GetTile(position.ToInt()));
        }

        public void Clear()
        {
            if (Tilemap.Count == 0) return;
            foreach(TileBundleGroup group in Tilemap)
            {
                bundleTileMap.RemoveGroup(group);
            }
            return;
        }
        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;

            tileMap = OwnerLayer.GetModule<TileMapModule>();
            bundleTileMap = OwnerLayer.GetModule<BundleTileMap>();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }
        
        private void ReplaceTile(TileBundleGroup tile)
        {
            RequestTileRemove(tile);
            RequestTilePaint(tile);
        }
        
        public override object Clone()
        {
            return new PopulationBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PopulationBehaviour;

            if (other == null) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private BundleCollection GetBundleCollection()
        {
            if (bundleCollection == null)
            {
                bundleCollection = LBSAssetMacro.LoadAssetByGuid<BundleCollection>(bundleRefGui);
            }

            return bundleCollection;
        }
        
        
        /// <summary>
        /// Get all tileBundleGroups that were rotated since the last time they were retrieved.
        /// The memory of new tiles will be cleared after calling this method.
        /// </summary>
        public TileBundleGroup[] RetrieveNewRotations()
        {
            // If null create a new one
            _newRotations ??= new HashSet<TileBundleGroup>();
            
            // Turn into array
            TileBundleGroup[] o = _newRotations.ToArray();
            
            // Clear memory
            _newRotations.Clear();
            
            // Return array
            return o;
        }
        
        #endregion
    }
}