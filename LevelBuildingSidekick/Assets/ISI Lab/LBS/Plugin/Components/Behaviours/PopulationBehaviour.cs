using System.Collections;
using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

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
        #endregion

        #region META-FIELDS
        [JsonIgnore]
        public Bundle selectedToSet;
        
        [JsonIgnore]
        public BundleCollection selectedCollectionToSet;
        
        [FormerlySerializedAs("selectedTypetoSet")] [JsonIgnore]
        public string selectedTypeFilter;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<TileBundleGroup> Tilemap => bundleTileMap.Groups;
        #endregion

        #region CONSTRUCTORS
        public PopulationBehaviour(Texture2D icon, string name) : base(icon, name) { }
        #endregion

        #region METHODS
        
        public override void OnGUI()
        {

        }

        public void AddTileGroup(Vector2Int position, BundleData bundle)
        {
            //Create group
            var group = bundleTileMap.CreateGroup(position, bundle, Vector2.right);

            //Add all tiles from the group
            foreach(LBSTile tile in group.TileGroup)
            {
                tileMap.AddTile(tile);
            }
        }

        public void AddTileGroup(Vector2Int position, Bundle bundle) => AddTileGroup(position, new BundleData(bundle));

        public void RemoveTileGroup(Vector2Int position)
        {
            var tile = tileMap.GetTile(position);
            var group = bundleTileMap.GetGroup(position);

            //CHANGE FROM HERE
            if (group != null)
            {
                foreach (var groupTile in group.TileGroup)
                {
                    tileMap.RemoveTile(tile);
                }
                bundleTileMap.RemoveGroup(group);
            }
        }

        public void SetBundle(TileBundleGroup group, Bundle bundle)
        {
            group.BundleData = new BundleData(bundle);
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
        }

        public Vector2 GetTileRotation(Vector2Int pos)
        {
            TileBundleGroup t = GetTileGroup(pos);
            return t.Rotation;
        }

        public BundleData GetBundleData(Vector2 position)
        {
            return GetBundleData(tileMap.GetTile(position.ToInt()));
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            Owner = layer;

            tileMap = Owner.GetModule<TileMapModule>();
            bundleTileMap = Owner.GetModule<BundleTileMap>();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            return new PopulationBehaviour(this.Icon, this.Name);
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

        #endregion
    }
}