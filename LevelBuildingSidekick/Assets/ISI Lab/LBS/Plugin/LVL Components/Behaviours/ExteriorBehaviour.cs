using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class ExteriorBehaviour : LBSBehaviour
{
    #region FIELDS

    #endregion

    #region CONSTRUCTORS
    public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    public override object Clone()
    {
        return new ExteriorBehaviour(this.icon, this.name);
    }

    public override void Init(LBSLayer layer)
    {

    }
}
