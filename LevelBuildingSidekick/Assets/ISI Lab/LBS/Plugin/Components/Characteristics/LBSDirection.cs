using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS;
using ISILab.LBS.Components;
using Newtonsoft.Json;
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
        [Tooltip("4-Conected: 0: Right, 1: Up, 2: Left, 3: Down")]
        [SerializeField, JsonRequired]
        private List<string> connections = new List<string>();

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<string> Connections => new List<string>(connections);

        [JsonIgnore]
        public int Size
        { 
            get => connections.Count;
            set
            {
                
                if(connections.Count < value)
                {
                    connections.AddRange(new string[value - connections.Count]);
                }
                else if(connections.Count > value)
                {
                    connections.RemoveRange(value - 1, value - connections.Count);
                }
            }
        }

        //[SerializeField]
        //public List<weightStruct> Weights => new List<weightStruct>(weights); 

        //public float TotalWeight => weights.Sum( w => w.weigth);
        #endregion

        #region CONSTRUCTORS
        public LBSDirection() : base() {  }
        

        public LBSDirection(List<string> tags)
        {
            this.connections = tags;
            Size = tags.Count;
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

        public void SetConnection(LBSTag tag, int index)
        {

            try
            {
                connections[index] = tag.Label;
            }
            catch
            {
                Debug.LogError("[ISILab] Index out of Range ");
                return;
            }
        }

        public override object Clone()
        {
            return new LBSDirection(new List<string>(this.connections));
        }

        public override bool Equals(object obj)
        {

            var other = obj as LBSDirection;

            if (other != null)
            {
                if (this.Size != other.Size) return false;
            }
            for(int i = 0; i>this.Size; i++)
            {
                if (this.Connections[i] != other.Connections[i]) { return false; }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override List<string> Validate()
        {
            List<string> warnings = new List<string>();
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] == null)
                {
                    warnings.Add("Connection " + i + " in LBSDirection is null.");
                }
            }
            
            return warnings;
        }
        #endregion

        
    }
}
