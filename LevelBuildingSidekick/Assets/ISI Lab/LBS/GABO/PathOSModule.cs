using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    //GABO TODO
    public class PathOSModule : LBSModule
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSTile> tiles = new List<LBSTile>();
        private Dictionary<Vector2Int, LBSTile> _tileDic = new Dictionary<Vector2Int, LBSTile>();
        #endregion


        //........

        public override void Clear()
        {
            Debug.Log("TERMINAR PathOSModule.Clear!!!");
        }

        public override object Clone()
        {
            Debug.Log("Ejecutando PathOSModule.Clone()");
            var clone = new PathOSModule();
            return clone;
        }

        public override bool IsEmpty()
        {
            Debug.Log("Ejecutando PathOSModule.IsEmpty()");
            return true;
        }
    }
}

