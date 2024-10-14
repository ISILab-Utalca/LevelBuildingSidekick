using ISILab.LBS.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    public class PathOSModule : LBSModule
    {
        public override void Clear()
        {
            Debug.Log("TERMINAR PathOSModule.Clear!!!");
        }

        public override object Clone()
        {
            Debug.Log("Ejecutando PathOSModule.Clone() por alguna razon");
            var clone = new PathOSModule();
            return clone;
        }

        public override bool IsEmpty()
        {
            Debug.Log("Ejecutando PathOSModule.IsEmpty() por alguna razon");
            return true;
        }
    }
}

