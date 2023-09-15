using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using LBS.Bundles;

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
        public LBSIdentifier Identifier => LBSAssetsStorage.Instance.Get<LBSIdentifier>().Find(s => s.Label == bundleTag);
        public List<LBSCharacteristic> Characteristics => new List<LBSCharacteristic>(characteristics);
        #endregion

        #region CONSTRUCTOR

        public BundleData()
        {
        }

        public BundleData(string bundle, List<LBSCharacteristic> characteristics)
        {
            this.bundleTag = bundle;
            foreach(var c in characteristics)
                this.characteristics.Add(c.Clone() as LBSCharacteristic);

        }

        public BundleData(Bundle bundle) : this(bundle.name, bundle.Characteristics)
        {
        }

        #endregion

        #region METHODS
        public object Clone()
        {
            return new BundleData(bundleTag, characteristics.Select(c => c.Clone() as LBSCharacteristic).ToList());
        }

        public override bool Equals(object obj)
        {
            if(obj is Bundle)
            {
                var b = obj as Bundle;
                return b.name == bundleTag;
            }
            return base.Equals(obj);
        }

        public LBSCharacteristic GetCharasteristic(Type type)
        {
            return characteristics.Find(c => c.GetType() == type);
        }

        public T GetCharasteristic<T>() where T : LBSCharacteristic
        {
            var type = typeof(T);
            return (T)characteristics.Find(c => c.GetType() == type);
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