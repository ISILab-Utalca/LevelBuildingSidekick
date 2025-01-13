using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{

    [System.Serializable]
    [RequieredModule(typeof(ConnectedTileMapModule))]
    public class SimpleConectedBehaviour : LBSBehaviour
    {
        public SimpleConectedBehaviour(Texture2D icon, string name) : base(icon, name) { }

        public override object Clone()
        {
            return new SimpleConectedBehaviour(this.Icon, this.Name);
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
        }
    }
}