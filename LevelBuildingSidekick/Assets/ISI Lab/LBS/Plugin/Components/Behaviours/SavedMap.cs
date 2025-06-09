using ISILab.LBS.Modules;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ISILab.LBS
{
    [System.Serializable]
    public class SavedMap : ScriptableObject, ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        public BundleTileMap map;
        [SerializeField, JsonRequired]
        public string mapName;
        [SerializeField, JsonRequired]
        public float savedScore;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public BundleTileMap Map => map;
        [JsonIgnore]
        public float Score => savedScore;
        [JsonIgnore]
        public string Name => mapName;
        
        #endregion

        #region CONSTRUCTOR
        public SavedMap(BundleTileMap map, string name, float score = 0)
        {
            this.map = map;
            this.mapName = name;
            this.savedScore = score;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region METHODS

        #endregion
    }
}