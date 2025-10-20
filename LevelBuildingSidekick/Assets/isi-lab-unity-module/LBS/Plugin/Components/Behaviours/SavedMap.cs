using ISILab.AI.Categorization;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ISILab.LBS
{
    [Serializable]
    public class SavedMap : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        protected BundleTilemapChromosome map;
        [SerializeField, JsonRequired]
        protected string mapName;
        [SerializeField, JsonRequired]
        protected float savedScore;
        [SerializeField]
        protected Texture2D image;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public BundleTilemapChromosome Map
        {
            get => map;
            set => map = value;
        }
        [JsonIgnore]
        public float Score
        {
            get => savedScore;
            set => savedScore = value;
        }
        [JsonIgnore]
        public string Name
        {
            get => mapName;
            set => mapName = value;
        }

        public Texture2D Image
        {
            get => image;
            set => image = value;
        }
        
        #endregion

        #region CONSTRUCTOR
        public SavedMap(BundleTilemapChromosome map, string name, float score = 0, Texture2D image = null)
        {
            this.map = map;
            this.mapName = name;
            this.savedScore = score;
            this.image = image;
        }

        public object Clone()
        {
            var clonedMap = this.map.Clone() as BundleTilemapChromosome;
            var savedMapClone = new SavedMap(clonedMap, mapName, savedScore, image);
            return savedMapClone;
        }

        #endregion

        #region METHODS

        #endregion
    }
}