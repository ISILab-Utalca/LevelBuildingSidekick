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
        protected List<LBSCharacteristic> characteristics;

        [SerializeField, JsonRequired]
        protected string bundleTag;

        #endregion

        #region PROPERTIES

        public string BundleTag => bundleTag;

        #endregion

        #region CONSTRUCTOR

        public BundleData()
        {
            characteristics = new List<LBSCharacteristic>();
        }

        public BundleData(string bundle, List<LBSCharacteristic> characteristics)
        {
            this.bundleTag = bundle;
            this.characteristics = characteristics;
        }

        public object Clone()
        {
            return new BundleData(bundleTag, characteristics.Select(c => c.Clone() as LBSCharacteristic).ToList());
        }

        #endregion

        #region METHODS

        #endregion
    }
}