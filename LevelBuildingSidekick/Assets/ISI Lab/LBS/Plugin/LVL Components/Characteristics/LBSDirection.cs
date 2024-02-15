using ISILab.Extensions;
using ISILab.LBS;
using ISILab.LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Directions", "")]
    public class LBSDirection : LBSCharacteristic, ICloneable
    {
        #region SUB-STRUCTURE
        /*
        [System.Serializable]
        public class weightStruct
        {
            [SerializeField]
            public GameObject target;

            [Range(0f, 1f)]
            public float weigth;
        };*/
        #endregion

        #region FIELDS
        [Tooltip("4-Conected: 0: R, 1: T, 2: L, 3: D")]
        [ScriptableObjectReference(typeof(LBSIdentifier))]
        [SerializeField, JsonRequired]
        private List<string> connections = new List<string>();

        #endregion

        #region PROPERTIES
        public List<string> Connections => new List<string>(connections);

        //[SerializeField]
        //public List<weightStruct> Weights => new List<weightStruct>(weights); 

        //public float TotalWeight => weights.Sum( w => w.weigth);
        #endregion

        #region CONSTRUCTORS
        public LBSDirection() : base() { }

        public LBSDirection(List<string> tags)
        {
            this.connections = tags;
        }
        #endregion

        #region METHODS
        public string[] GetConnection(int rotation = 0)
        {
            var conections = connections;
            var toR = new List<string>(connections);

            toR = toR.Rotate(rotation);

            return toR.ToArray();
        }

        public override object Clone()
        {
            return new LBSDirection(new List<string>(this.connections));
        }

        public override bool Equals(object obj)
        {
            return false; // TODO: Implement
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
