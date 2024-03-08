using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    public class LBSRotationCharacteristic : LBSCharacteristic
    {
        [SerializeField, JsonRequired]
        Vector2 rotation = Vector2.right;

        [JsonIgnore]
        public Vector2 Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public LBSRotationCharacteristic() : base()
        {

        }

        public LBSRotationCharacteristic(Vector2 vector)
        {
            rotation = vector;
        }

        public override object Clone()
        {
            return new LBSRotationCharacteristic(rotation);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as LBSRotationCharacteristic;

            return other.rotation.Equals(this.rotation);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}