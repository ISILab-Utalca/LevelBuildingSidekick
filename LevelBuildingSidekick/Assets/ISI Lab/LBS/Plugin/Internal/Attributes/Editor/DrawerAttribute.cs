using ISILab.LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerAttribute : LBSAttribute
    {
        public Type type;

        public DrawerAttribute(Type t)
        {
            type = t;
        }
    }
}