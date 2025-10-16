using LBS.Bundles;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ISILab.LBS.Modules.ConnectedTileMapModule;

namespace ISILab.LBS.Characteristics
{
    [Serializable]
    [LBSCharacteristic("Directioned Chance", "Define chances based on direction")]

    public class LBSDirectionedChance : LBSCharacteristic, ICloneable
    {
        [SerializeField]
        public class TileDirectionChance
        {
            [SerializeField]
            public Bundle target;

            //[SerializeField]
            //public List<string> target;

            [Range(0f, 1f)]
            public float chance;
        }

        [SerializeField]
        public class TileDirection
        {
            [SerializeField]
            public Bundle mainTarget;

            //[SerializeField]
            //public List<string> mainTarget;

            [SerializeField]
            public List<int> direction;

            [SerializeField]
            public List<TileDirectionChance> chances = new List<TileDirectionChance>();
        }

        [SerializeField]
        public List<TileDirection> tileDirections = new List<TileDirection>();

        [SerializeField]
        public ConnectedTileType currentType = ConnectedTileType.EdgeBased;

        public override void OnEnable()
        {
            //Owner.ClearEvents();
            //Owner.OnAddChild += OnAddChildToOwner;
            //Owner.OnRemoveChild += OnRemoveChildToOwner;

            _Update();
        }

        public void _Update()
        {
            if (Owner == null)
                return;

            var bundles = Owner.ChildsBundles;

            while (bundles.Count < tileDirections.Count)
            {
                for (int i = 0; i < tileDirections.Count; i++)
                {
                    if (!bundles.Equals(tileDirections[i].mainTarget))
                    {
                        tileDirections.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int i = 0; i < bundles.Count; i++)
            {
                if (i == tileDirections.Count)
                    tileDirections.Add(new TileDirection() { mainTarget = bundles[i] });

                if (bundles[i] != null && !bundles[i].Equals(tileDirections[i].mainTarget))
                {
                    tileDirections[i].mainTarget = bundles[i];
                }
            }

        }

        public override object Clone()
        {
            var childs = Owner.ChildsBundles;
            return new LBSDirectionedGroup();
        }

        public override bool Equals(object obj)
        {
            return false; // TODO: implement this method
        }

        public override List<string> Validate()
        {
            //throw new System.NotImplementedException();
            return base.Validate();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}


