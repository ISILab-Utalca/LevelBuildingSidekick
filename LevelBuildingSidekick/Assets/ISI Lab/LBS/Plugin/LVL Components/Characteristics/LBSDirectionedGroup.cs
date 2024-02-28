using ISILab.LBS;
using ISILab.LBS.Characteristics;
using LBS.Bundles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;


namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristicAttribute("Connection group", "")]
    public class LBSDirectionedGroup : LBSCharacteristic, ICloneable
    {
        #region SUB-STRUCTURE
        [System.Serializable]
        public class WeightStruct
        {
            [SerializeField]
            public Bundle target;

            [Range(0f, 1f)]
            public float weigth;
        };
        #endregion

        [SerializeField]
        public List<WeightStruct> Weights = new List<WeightStruct>();

        [JsonIgnore]
        public Action OnAddOwnerChild;
        [JsonIgnore]
        public Action OnRemoveOwnerChild;

        public LBSDirectionedGroup()
        {

        }

        public override void OnEnable()
        {
            Owner.OnAddChild += OnAddChildToOwner;
            Owner.OnRemoveChild += OnRemoveChildToOwner;

            Update();
        }

        public void Update()
        {
            if (Owner == null)
                return;

            var bundles = Owner.ChildsBundles;

            while(bundles.Count < Weights.Count)
            {
                for (int i = 0; i < Weights.Count; i++)
                {
                    if (!bundles.Equals(Weights[i].target))
                    {
                        Weights.RemoveAt(i);
                        break;
                    }
                }
            }
            

            for(int i = 0; i < bundles.Count; i++)
            {
                if (i == Weights.Count)
                    Weights.Add(new WeightStruct() { target = bundles[i], weigth = 0.5f });

                if (!bundles[i].Equals(Weights[i].target))
                {
                    Weights[i].target = bundles[i];
                }
            }

            
        }

        private void OnAddChildToOwner(Bundle child)
        {
            var c = new LBSDirection();
            child.AddCharacteristic(c);
            Weights.Add(new WeightStruct() { target = child, weigth = 0.5f });
        }

        private void OnRemoveChildToOwner(Bundle child)
        {
            var w = Weights.Find(w => w.target.Equals(child));

            Weights.Remove(w);
        }

        public override object Clone()
        {
            var childs = Owner.ChildsBundles;
            return new LBSDirectionedGroup();
        }

        public List<LBSDirection> GetDirs()
        {
            var r = new List<LBSDirection>();
            foreach (var w in Weights)
            {
                r.Add(w.target.GetCharacteristics<LBSDirection>()[0]);
            }
            return r;
        }

        public override bool Equals(object obj)
        {
            return false; // TODO: implement this method
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