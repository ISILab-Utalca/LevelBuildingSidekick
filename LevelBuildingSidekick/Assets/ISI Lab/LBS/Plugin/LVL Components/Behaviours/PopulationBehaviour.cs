using LBS.Behaviours;
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

    #region CONSTRUCTORS
    public PopulationBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public override object Clone()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
