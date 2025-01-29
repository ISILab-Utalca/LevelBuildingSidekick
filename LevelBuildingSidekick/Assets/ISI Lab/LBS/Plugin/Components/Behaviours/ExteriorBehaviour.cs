using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.Commons;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using JetBrains.Annotations;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS.Behaviours
{
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule),
                    typeof(ConnectedTileMapModule))]
    public class ExteriorBehaviour : LBSBehaviour
    {
        #region FIELDS
        [SerializeField, InspectorName("Target Bundle")]
        private Bundle targetBundleRef;
        
        // Stores the guid for the object instead of the current path
        [FormerlySerializedAs("targetBundle")] [JsonRequired, SerializeField, HideInInspector]
        private string bundlePath = "";

        /***
         * Use asset's GUID; current bundle:
         * - "Exterior_Plains"
         */
        private string defaultBundleGuid = "9d3dac0f9a486fd47866f815b4fefc29"; 
        #endregion

        #region META-FIELDS
        [JsonIgnore]
        public LBSTag identifierToSet;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        private TileMapModule TileMap => Owner.GetModule<TileMapModule>();

        [JsonIgnore]
        private ConnectedTileMapModule Connections => Owner.GetModule<ConnectedTileMapModule>();

        [JsonIgnore]
        public string BundlePath
        {
            get
            {
                if(bundlePath == "") return AssetDatabase.GUIDToAssetPath(defaultBundleGuid);
                return bundlePath;
            }
            set => bundlePath = value;
        }

        public Bundle Bundle
        {
            get
            {
                if(bundlePath != "") 
                {
                    var bundle = AssetDatabase.LoadAssetAtPath<Bundle>(bundlePath); // The custom bundle
                    //Debug.Log(bundle);
                    return bundle;
                }
          
                var guiBundle = AssetDatabase.LoadAssetAtPath<Bundle>(AssetDatabase.GUIDToAssetPath(defaultBundleGuid)); // the default bundle
                Debug.Log(guiBundle);
                return guiBundle;

            }
            set => targetBundleRef = value;
        }

        public Bundle TargetBundleRef
        {
            get => targetBundleRef;
            set => targetBundleRef = value;
        }

        [JsonIgnore]
        public List<LBSTile> Tiles => TileMap.Tiles;

        [JsonIgnore]
        public List<Vector2Int> Directions => ISILab.Commons.Directions.Bidimencional.Edges;
        #endregion
        
        #region CONSTRUCTORS

        public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name)
        {
            OnGUI();
        }
        
        #endregion

        #region METHODS

        // Method invoked from the LBSLayer Class, whenever the scriptable object's values are modified
        public sealed override void OnGUI()
        {
            if (!targetBundleRef)
            {
                bundlePath = AssetDatabase.GUIDToAssetPath(defaultBundleGuid);
            }
            else
            {
                bundlePath = AssetDatabase.GetAssetPath(targetBundleRef);
                defaultBundleGuid = AssetDatabase.AssetPathToGUID(bundlePath);
            }
            targetBundleRef = AssetDatabase.LoadAssetAtPath<Bundle>(bundlePath);
        }


        public override void OnAttachLayer(LBSLayer layer)
        {
            Owner = layer;
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
            Owner.GetModule<TileMapModule>().RemoveTile(tile);
            Owner.GetModule<ConnectedTileMapModule>().RemoveTile(tile);
        }

        public void AddTile(LBSTile tile)
        {
            Owner.GetModule<TileMapModule>()
                .AddTile(tile);

            Owner.GetModule<ConnectedTileMapModule>()
                .AddPair(tile, new List<string> { "", "", "", "" }, new List<bool> { false, false, false, false });
        }

        public void SetConnection(LBSTile tile, int direction, string connection, bool canEditedByAI)
        {
            var t = Owner.GetModule<ConnectedTileMapModule>().GetPair(tile);
            t.SetConnection(direction, connection, canEditedByAI);
        }

        public List<string> GetConnections(LBSTile tile)
        {
            return Connections.GetConnections(tile);
        }

        public override object Clone()
        {
            return new ExteriorBehaviour(this.Icon, this.Name);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExteriorBehaviour;

            if (other == null) return false;

            if (!this.bundlePath.Equals(other.bundlePath)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        
        #endregion
        
        
    }
}