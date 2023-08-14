using LBS.Behaviours;
using LBS.Components;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class ExteriorBehaviour : LBSBehaviour
{
    #region FIELDS
    [JsonRequired, SerializeField]
    private string aaa = "a";
    #endregion

    #region CONSTRUCTORS
    public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    public override object Clone()
    {
        return new ExteriorBehaviour(this.Icon, this.Name);
    }

    public override void Init(LBSLayer layer)
    {
        base.Init(layer);
    }
}
