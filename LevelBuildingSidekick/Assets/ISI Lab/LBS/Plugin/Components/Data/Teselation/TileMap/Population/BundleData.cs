using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.Macros;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEngine;

namespace LBS.Components.TileMap // FIX: change namespace to ISILab.LBS.Bundle
{
    [System.Serializable]
    public class BundleData : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSCharacteristic> characteristics = new List<LBSCharacteristic>();

        [SerializeField, HideInInspector, JsonRequired]
        protected string guid;

        [SerializeField, JsonRequired]
        protected string bundleName;

        [SerializeField, SerializeReference]
        protected Bundle bundle;
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string GUID
        {
            get
            {
                if(string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning($"No GUID stored for this Bundle Data: {bundleName}");
                    //guid = LBSAssetMacro.GetGuidFromAsset(bundle);
                }
                return guid;
            }
        }

        [JsonIgnore]
        public string BundleName => bundleName;

        [JsonIgnore]
        public Bundle Bundle
        {
            get
            {
                if (bundle == null)
                {
                    bundle = LBSAssetMacro.LoadAssetByGuid<Bundle>(GUID);
                    if (bundle == null)
                        bundle = LBSAssetsStorage.Instance.Get<Bundle>().Find(b => b.name == bundleName); // For compatibility
                }
                    
                return bundle;
            }
        }

        [JsonIgnore]
        public List<LBSCharacteristic> Characteristics => new List<LBSCharacteristic>(characteristics);
        #endregion

        #region CONSTRUCTOR
        public BundleData()
        {
        }

        public BundleData(string bundle, string guid, List<LBSCharacteristic> characteristics)
        {
            this.bundleName = bundle;
            this.characteristics = characteristics;
            this.guid = guid;
            //Debug.Log($"Bundle Data ({bundle}) Constructed.");
        }

        public BundleData(Bundle bundle) : this(bundle.name, bundle.GUID, bundle.Characteristics)
        {
            this.bundle = bundle;
            //Debug.Log($"Bundle Data ({bundle.name}) Constructed.");
        }
        #endregion

        #region METHODS
        public object Clone()
        {
            return new BundleData(bundleName, GUID, characteristics.Select(c => c.Clone() as LBSCharacteristic).ToList());
        }

        /*
        public override bool Equals(object obj)
        {
            if(obj is Bundle)
            {
                var b = obj as Bundle;
                return b.name == bundleName;
            }
            return base.Equals(obj);
        }*/

        public LBSCharacteristic GetCharacteristic(Type type)
        {
            return characteristics.Find(c => c.GetType() == type);
        }

        public T GetCharacteristic<T>() where T : LBSCharacteristic
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
        
        public override bool Equals(object obj)
        {
            var other = obj as BundleData;
            var characteristics = new List<LBSCharacteristic>();

            if (other == null)
            {
                var bundle = obj as Bundle;
                
                if(bundle == null)
                    return false;
                
                if(!this.bundleName.Equals(bundle.name))
                    return false;

                if (this.characteristics.Count != other.characteristics.Count)
                    return false;

                characteristics = bundle.Characteristics;

            }
            else
            {
                if (!this.bundleName.Equals(other.bundleName)) 
                    return false;

                if (this.characteristics.Count != other.characteristics.Count)
                    return false;

                characteristics = other.characteristics;
            }

            var cCount = characteristics.Count;

            for (int i = 0; i < cCount; i++)
            {
                var c1 = this.characteristics[i];
                var c2 = characteristics[i];

                if (!c1.Equals(c2)) return false;
            }
            return true;
        }
        #endregion
    }
}