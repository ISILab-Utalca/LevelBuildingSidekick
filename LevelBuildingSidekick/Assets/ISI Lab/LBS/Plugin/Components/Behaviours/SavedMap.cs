using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    public class SavedMap
    {
        #region FIELDS
        [SerializeField, JsonIgnore]
        protected BundleTileMap map;
        [SerializeField]
        protected string mapName;
        [SerializeField]
        protected float savedScore;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public BundleTileMap Map => map;
        [JsonIgnore]
        public float Score => savedScore;
        public string Name => mapName;
        
        #endregion

        #region CONSTRUCTOR
        public SavedMap(BundleTileMap map, string name, float score = 0)
        {
            this.map = map;
            this.mapName = name;
            this.savedScore = score;
        }
        #endregion

        #region METHODS

        #endregion
    }
}