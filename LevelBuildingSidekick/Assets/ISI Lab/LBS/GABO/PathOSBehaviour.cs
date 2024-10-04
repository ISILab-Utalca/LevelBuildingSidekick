using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    public class PathOSBehaviour : LBSBehaviour
    {
        #region FIELDS
        [SerializeField, JsonIgnore]
        TileMapModule tileMap;
        #endregion

        public PathOSBehaviour(Texture2D icon, string name) : base(icon, name)
        {

        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }
    }
}
