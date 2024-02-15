using ISILab.LBS.Internal;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Tags", "")]
    public class LBSTagsCharacteristic : LBSCharacteristic
    {
        [JsonRequired]
        string tagName = "";

        [SerializeField, JsonIgnore]
        protected LBSIdentifier value;

        [JsonIgnore]
        public LBSIdentifier Value
        {
            get
            {
                if (value == null)
                    value = LBSAssetsStorage.Instance.Get<LBSIdentifier>().Find(i => i.Label == tagName);
                return value;
            }
            set
            {
                this.value = value;
                tagName = value.Label;
            }
        }

        public LBSTagsCharacteristic(LBSIdentifier value)
        {
            this.value = value;
            if (value != null)
                tagName = value.Label;
        }

        public LBSTagsCharacteristic()
        {
            this.value = null;
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
            if (ch.value != this.value)
                return false;
            return true;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}