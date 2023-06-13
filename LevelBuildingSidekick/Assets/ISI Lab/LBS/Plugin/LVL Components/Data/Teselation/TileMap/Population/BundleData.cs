using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class BundleData : ICloneable
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSCharacteristic> characteristics = new List<LBSCharacteristic>();

        [SerializeField, JsonRequired]
        protected string bundleTag;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string BundleTag => bundleTag;

        #endregion

        #region CONSTRUCTOR

        public BundleData()
        {
        }

        public BundleData(string bundle, List<LBSCharacteristic> characteristics)
        {
            this.bundleTag = bundle;
            this.characteristics = characteristics;
        }
        #endregion

        #region METHODS
        public object Clone()
        {
            return new BundleData(bundleTag, characteristics.Select(c => c.Clone() as LBSCharacteristic).ToList());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
        #endregion
    }
}