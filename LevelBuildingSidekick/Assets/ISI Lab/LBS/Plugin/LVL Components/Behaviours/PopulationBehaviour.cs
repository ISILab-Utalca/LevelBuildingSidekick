using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(TileMapModule), typeof(BundleTileMap))]
public class PopulationBehaviour : LBSBehaviour
{
    #region FIELDS

    [JsonIgnore]
    TileMapModule tileMap;
    [JsonIgnore]
    BundleTileMap bundleTile;

    #endregion




    public override object Clone()
    {
        throw new System.NotImplementedException();
    }
}
