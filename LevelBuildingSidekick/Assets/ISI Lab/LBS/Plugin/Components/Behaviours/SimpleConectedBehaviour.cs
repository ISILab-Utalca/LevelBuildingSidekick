using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{

    [System.Serializable]
    [RequieredModule(typeof(ConnectedTileMapModule))]
    public class SimpleConectedBehaviour : LBSBehaviour
    {
        public SimpleConectedBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint) { }

        public override object Clone()
        {
            return new SimpleConectedBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override void OnGUI()
        {
        }
        
        public override void OnAttachLayer(LBSLayer layer)
        {
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
        }
    }
}