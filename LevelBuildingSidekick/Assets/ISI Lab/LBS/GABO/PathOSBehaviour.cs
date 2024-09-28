using ISILab.LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    public class PathOSBehaviour : LBSBehaviour
    {
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
