using System;
using System.Collections.Generic;
using UnityEngine;
using LBS.Bundles;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using UnityEditor.MemoryProfiler;
using ISILab.Extensions;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Connection Grid", "")]
    public class LBSTerrainConnectionGrid : LBSCharacteristic, ICloneable
    {
        [SerializeField, JsonRequired]
        Dictionary<Asset, AssetConnectionGrid> gridList;
        [SerializeField, JsonRequired]
        Dictionary<int, UnityEngine.Color> flagColorPalette;
        int gridSize;

        #region PROPERTIES
        [JsonIgnore]
        public List<Asset> Assets
        {
            get => Owner.Assets;
        }

        [JsonIgnore]
        public Dictionary<Asset, AssetConnectionGrid> GridList
        {
            get => gridList;
            set => gridList = value;
        }
        public int GridSize => gridSize;

        #endregion

        #region CONSTRUCTOR
        public LBSTerrainConnectionGrid(int gSize = 9) {
            gridSize = gSize;
            gridList = new Dictionary<Asset, AssetConnectionGrid>();
            for(int i = 0; i<Assets.Count; i++)
            {
                gridList[Assets[i]] = new AssetConnectionGrid(gridSize);
            }
        }
        #endregion

        #region METHODS
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var other = obj as LBSTerrainConnectionGrid;
            if(other!=null)
            {
                if (this.Assets.Count != other.Assets.Count) return false;
                if (this.gridSize != other.gridSize) return false;
                if (this.gridList.Count != other.gridList.Count) return false;
            }
            for (int i = 0; i < Assets.Count; i++)
            {
                if (Assets[i].Equals(other.Assets[i])) return false;
            }
            foreach (Asset asset in Assets)
            {
                if (gridList[asset].Equals(other.gridList[asset])) return false;
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
            if (Assets.Count == 0) {
                warnings.Add("Bundle contains no assets");
                return warnings; 
            }
            foreach(Asset asset in Assets)
            {
                if (gridList[asset] == null)
                {
                    warnings.Add("gridlist for " + asset + " is null.");
                }
            }

            return warnings;
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

        public int VectorToInt(Vector2 vector)
        {
            //If not a square, return
            var lengthSqrt = Mathf.Sqrt(TerrainFlag.Length);
            if (lengthSqrt != Mathf.RoundToInt(lengthSqrt))
            {
                throw new Exception("Terrain flag is not a square!");
            }
            //If over length, return
            if ((vector.x * vector.y) > (terrainFlag.Length)) { return -1; }
            var vecInt = vector.ToInt();

            //This will likely explode in my face in the future and ONLY works if the terrain is a square. Oh well!
            return vecInt.y * Mathf.RoundToInt(lengthSqrt) + vecInt.x;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals (object obj)
        {
            var other = obj as AssetConnectionGrid;
            if (terrainFlag.Length != other.terrainFlag.Length) return false;
            for(int i=0; i<terrainFlag.Length;i++)
            {
                if (terrainFlag[i] != other.terrainFlag[i]) return false;
            }
            return true;
        }
    }
}

