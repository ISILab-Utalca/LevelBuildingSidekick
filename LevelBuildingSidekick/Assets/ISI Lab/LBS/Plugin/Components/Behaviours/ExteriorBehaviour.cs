using System;
using System.Collections.Generic;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    [Serializable]
    [RequieredModule(typeof(TileMapModule),
                    typeof(ConnectedTileMapModule))]
    public class ExteriorBehaviour : LBSBehaviour
    {
        #region FIELDS
        [JsonProperty, SerializeReference, SerializeField]
        private Bundle targetBundleRef;

        /***
         * Use asset's GUID; current bundle:
         * - "Exterior_Plains"
         */
        [SerializeField]
        private string bundleRefGui = "9d3dac0f9a486fd47866f815b4fefc29";
        #endregion

        #region META-FIELDS
        [JsonIgnore]
        public LBSTag identifierToSet;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        private TileMapModule TileMap => OwnerLayer.GetModule<TileMapModule>();

        [JsonIgnore]
        private ConnectedTileMapModule Connections => OwnerLayer.GetModule<ConnectedTileMapModule>();
        
        public Bundle Bundle
        {
            get => GetBundleRef();
            set
            {
                targetBundleRef = value;
                bundleRefGui = LBSAssetMacro.GetGuidFromAsset(value);
            }
        }

        [JsonIgnore]
        public List<LBSTile> Tiles => TileMap.Tiles;

        [JsonIgnore]
        public List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;
        #endregion
        
        #region CONSTRUCTORS

        public ExteriorBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
            OnGUI();
        }
        
        #endregion

        #region METHODS

        // Method invoked from the LBSLayer Class, whenever the scriptable object's values are modified
        public sealed override void OnGUI()
        {
            GetBundleRef();
        }

        public Bundle GetBundleRef()
        {
            if (bundleRefGui != null)
            {
                // either loads the default guid or the saved guid field
                targetBundleRef = LBSAssetMacro.LoadAssetByGuid<Bundle>(bundleRefGui);
            }
            
            return targetBundleRef;
        }


        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new NotImplementedException();
        }

        public LBSTile GetTile(Vector2Int pos)
        {
            return TileMap.GetTile(pos);
        }

        public void RemoveTile(LBSTile tile)
        {
            RequestTileRemove(tile);
            OwnerLayer.GetModule<TileMapModule>().RemoveTile(tile);
            OwnerLayer.GetModule<ConnectedTileMapModule>().RemoveTile(tile);
        }

        public void AddTile(LBSTile tile)
        {
            RequestTilePaint(tile);
            
            OwnerLayer.GetModule<TileMapModule>()
                .AddTile(tile);

            OwnerLayer.GetModule<ConnectedTileMapModule>()
                .AddPair(tile, new List<string> { "", "", "", "" }, new List<bool> { false, false, false, false });
        }

        public void SetConnection(LBSTile tile, int direction, string connection, bool canEditedByAI)
        {
            var t = OwnerLayer.GetModule<ConnectedTileMapModule>().GetPair(tile);
            t.SetConnection(direction, connection, canEditedByAI);
            ReplaceTile(tile);
        }

        public List<string> GetConnections(LBSTile tile)
        {
            return Connections.GetConnections(tile);
        }
        
        private void ReplaceTile(LBSTile tile)
        {
            RequestTilePaint(tile);
            LBSTile old = TileMap.GetTile(tile.Position);
            if (old != null)
            {
                RequestTileRemove(old);
            }
        }
        

        public override object Clone()
        {
            return new ExteriorBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExteriorBehaviour;

            if (other == null) return false;
            
            //if (!GetBundleRef().Equals(other.GetBundleRef())) return false;
            if (!Equals(GetBundleRef(), other.GetBundleRef())) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        
        #endregion



    }
}