using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISILab.Macros;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Tags", "")]
    public class LBSTagsCharacteristic : LBSCharacteristic, ISerializationCallbackReceiver
    {
        [SerializeField, JsonRequired]
        string tagName = "";

        [SerializeField, JsonRequired/*, JsonIgnore*/]
        protected LBSTag value;

        [SerializeField, JsonRequired]
        protected string tagGUID = "";

        [JsonIgnore]
        public LBSTag Value
        {
            get
            {
                if (value == null)
                    //value = LBSAssetsStorage.Instance.Get<LBSTag>().Find(i => i.Label == tagName);
                    value = LBSAssetMacro.LoadAssetByGuid<LBSTag>(tagGUID);
                return value;
            }
            set
            {
                this.value = value;
                tagName = value.Label;
            }
        }

        [JsonIgnore]
        public string TagGUID
        {
            get
            {
                string s = "";
                if (string.IsNullOrEmpty(tagGUID))
                {
                    tagGUID = LBSAssetMacro.GetGuidFromAsset(value);
                    s += $"Null or Empty Tag GUID -> Calling GetGuidFromAsset( {value} )\n"; // Por alguna razon hay veces en que 'value' se muestra como Material??? Pero no pasa con bundles de population
                }
                s += $"Tag GUID = {tagGUID}";
                //Debug.Log(s);
                return tagGUID;
            }
        }

        public LBSTagsCharacteristic(LBSTag value)
        {
            //Debug.Log("CONSTRUCTOR 1 PARAMETRO INVOCADO [" + value + "]");
            this.value = value;
            if (value != null)
                tagName = value.Label;
        }

        public LBSTagsCharacteristic()
        {
            //Debug.Log("CONSTRUCTOR SIN PARAMETROS INVOCADO [Value: " + value + ", TagName: " + tagName + "]");

            //Value = LBSAssetMacro.LoadAssetByGuid<LBSTag>(tagGUID);
            if (value != null)
                tagName = value.Label; ;
            //this.value = null;
        }

        public override object Clone()
        {
            return new LBSTagsCharacteristic(this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is LBSTagsCharacteristic))
                return false;
            var ch = (LBSTagsCharacteristic)obj;
            //if((ch.value == null && value != null) || (ch.value != null && value == null))
            //    return false;
            //if (ch.value == null && value == null)
            //    return true;
            //return ch.value.Equals(value);
            return object.Equals(ch.value, value);
            //if (ch.value != this.value)
            //    return false;
            //return true;
        }

        public override string ToString()
        {
            return tagName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override List<string> Validate()
        {
            List<string> warnings = new List<string>();

            if (value == null)
            {
                warnings.Add("The tag in LBSTagsCharacteristic is null.");
            }
            
            return warnings;
        }

        public void OnBeforeSerialize()
        {
            //Debug.Log("Before Deserialize");
            var _ = TagGUID;
            if(value != null)
                tagName = value.label;
        }

        public void OnAfterDeserialize()
        {
            //Debug.Log("After Deserialize");
        }
    }
}