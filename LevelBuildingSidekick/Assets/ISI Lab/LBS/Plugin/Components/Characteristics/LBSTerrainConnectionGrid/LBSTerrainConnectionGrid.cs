using System;
using System.Collections.Generic;
using UnityEngine;
using LBS.Bundles;
using System.Linq;
using System.Drawing;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Connection Grid", "")]
    public class LBSTerrainConnectionGrid : LBSCharacteristic, ICloneable
    {
        Dictionary<Asset, AssetConnectionGrid> gridList;
        Dictionary<int, UnityEngine.Color> flagColorPalette; 

        #region PROPERTIES
        public List<Asset> Assets
        {
            get => Owner.Assets;
        }
        public Dictionary<Asset, AssetConnectionGrid> GridList
        {
            get => gridList;
            set => gridList = value;
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

        public AssetConnectionGrid(int[] terrainFlag)
        {
            this.terrainFlag = terrainFlag;
        }
        public AssetConnectionGrid(int q)
        {
            terrainFlag = new int[q];
            for(int i=0; i<q; i++)
            {
                terrainFlag[i] = 0;
            }
        }

        public int VectorToIntConversion(Vector2 vector)
        {
            //If over 9, return
            if ((vector.x * vector.y) > (terrainFlag.Length)) { return -1; }

            throw new NotImplementedException();

        }
    }
}

