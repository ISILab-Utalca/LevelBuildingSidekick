using System;
using System.Collections.Generic;
using UnityEngine;
using LBS.Bundles;
using System.Linq;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Connection Grid", "")]
    public class LBSTerrainConnectionGrid : LBSCharacteristic, ICloneable
    {
        public Dictionary<Asset, AssetConnectionGrid> gridList;
        #region PROPERTIES
        public List<Asset> Assets
        {
            get => Owner.Assets;
        }
        #endregion

        #region CONSTRUCTOR
        public LBSTerrainConnectionGrid() {

        }
        #endregion

        #region METHODS
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override List<string> Validate()
        {
            throw new NotImplementedException();
        }
        #endregion

    }

    public class AssetConnectionGrid
    {
        public int[] terrainFlag = new int[9];

        public int[] TerrainFlag
        {
            get => terrainFlag;
        }

        public int VectorToIntConversion(Vector2 vector)
        {
            //If over 9, return
            if ((vector.x * vector.y) > (terrainFlag.Length)) { return -1; }

            throw new NotImplementedException();

        }
    }
}

