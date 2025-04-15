using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    public class LBSPathOSTagsCharacteristic : LBSCharacteristic
    {
        [JsonRequired]
        string tagName = "";

        [SerializeField, JsonIgnore]
        protected PathOSTag value;

        [JsonIgnore]
        public PathOSTag Value
        {
            get
            {
                if (value == null)
                    value = LBSAssetsStorage.Instance.Get<PathOSTag>().Find(i => i.Label == tagName);
                return value;
            }
            set
            {
                this.value = value;
                tagName = value.Label;
            }
        }

        public LBSPathOSTagsCharacteristic(PathOSTag value)
        {
            this.value = value;
            if (value != null)
                tagName = value.Label;
        }

        public LBSPathOSTagsCharacteristic()
        {
            this.value = null;
        }

        public override object Clone()
        {
            return new LBSPathOSTagsCharacteristic(this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is LBSPathOSTagsCharacteristic))
                return false;
            var ch = (LBSPathOSTagsCharacteristic)obj;
            if (ch.value != this.value)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}